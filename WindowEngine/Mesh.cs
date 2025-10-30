using Assimp;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace WindowEngine;
public class Mesh
{
    private readonly float[] _vertices;
    private int _vericesLength;
    private int _vao;
    private int _vbo;
    private Texture _diffuseMap;
    private Shader _shader;

    private Camera _camera;

    public String Name;
    
    public Matrix4 Transform = Matrix4.Identity;

    public Mesh(string name, string fbxPath, Shader shader, Texture diffuseMap, Camera camera)
    {
        _shader = shader;
        _diffuseMap = diffuseMap;
        
        _camera = camera;

        Name = name;
        
        _vertices = LoadFbx(fbxPath);
        _vericesLength = _vertices.Length;
        _vbo = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
        GL.BufferData(BufferTarget.ArrayBuffer, this._vertices.Length * sizeof(float), this._vertices, BufferUsageHint.StaticDraw);
        
        {
            _vao = GL.GenVertexArray();
            GL.BindVertexArray(_vao);

            var positionLocation = _shader.GetAttribLocation("aPos");
            GL.EnableVertexAttribArray(positionLocation);
            GL.VertexAttribPointer(positionLocation, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 0);

            var normalLocation = _shader.GetAttribLocation("aNormal");
            GL.EnableVertexAttribArray(normalLocation);
            GL.VertexAttribPointer(normalLocation, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 3 * sizeof(float));
            
            var texCoordLocation = _shader.GetAttribLocation("aTexCoords");
            GL.EnableVertexAttribArray(texCoordLocation);
            GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, 8 * sizeof(float), 6 * sizeof(float));
        }
    }

    private float[] LoadFbx(string path)
    {
        var importer = new AssimpContext();
        var scene = importer.ImportFile(path, PostProcessSteps.Triangulate | PostProcessSteps.FlipUVs | PostProcessSteps.GenerateNormals);

        if (scene.MeshCount == 0)
            throw new Exception("FBX contains no meshes!");

        var mesh = scene.Meshes[0]; // Take first mesh
        float[] vertices = new float[mesh.VertexCount * 8];

        for (int i = 0; i < mesh.VertexCount; i++)
        {
            var v = mesh.Vertices[i];
            var n = mesh.Normals[i];
            var t = mesh.TextureCoordinateChannels[0].Count > 0 ? mesh.TextureCoordinateChannels[0][i] : new Vector3D(0, 0, 0);

            vertices[i * 8 + 0] = v.X;
            vertices[i * 8 + 1] = v.Y;
            vertices[i * 8 + 2] = v.Z;

            vertices[i * 8 + 3] = n.X;
            vertices[i * 8 + 4] = n.Y;
            vertices[i * 8 + 5] = n.Z;

            vertices[i * 8 + 6] = t.X;
            vertices[i * 8 + 7] = 1 - t.Y;
        }

        return vertices;
    }
    
    public void Draw(Vector3 diffuseColor, Shader shader)
    {
        GL.BindVertexArray(_vao);
        
        _diffuseMap.Use(TextureUnit.Texture0);
        
        shader.Use();

        shader.SetMatrix4("model", Transform);
        shader.SetMatrix4("view", _camera.GetViewMatrix());
        shader.SetMatrix4("projection", _camera.GetProjectionMatrix());
        
        shader.SetInt("material.diffuse", 0);

        shader.SetVector3("light.position", new Vector3(-2f, 3f, -8f));
        shader.SetVector3("light.ambient", new Vector3(0.2f));
        shader.SetVector3("light.diffuse", diffuseColor);
                
        GL.DrawArrays(OpenTK.Graphics.OpenGL4.PrimitiveType.Triangles, 0, _vericesLength / 8);
    }
}
