using System;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;
using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.Common;

namespace WindowEngine
{
    public class Game : GameWindow
    {
        private int vertexBufferHandle;
        private int shaderProgramHandle;
        private int vertexArrayHandle;

        private int modelLoc, viewLoc, projLoc;

        private int vertexCount = 36;
        private float angle;
        
        public Game()
            : base(GameWindowSettings.Default, NativeWindowSettings.Default)
        {
            this.Size = new Vector2i(1280, 768);
            this.CenterWindow(this.Size);
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            GL.Viewport(0, 0, e.Width, e.Height);
            base.OnResize(e);
        }

        protected override void OnLoad()
        {
            base.OnLoad();

            GL.ClearColor(new Color4(0.5f, 0.7f, 0.8f, 1f));
            
            float[] vertices = {
                // front
                -0.5f,-0.5f, 0.5f,  1,0,0,   0.5f,-0.5f, 0.5f,  1,0,0,   0.5f, 0.5f, 0.5f,  1,0,0,
                -0.5f,-0.5f, 0.5f,  1,0,0,   0.5f, 0.5f, 0.5f,  1,0,0,  -0.5f, 0.5f, 0.5f,  1,0,0,

                // right
                0.5f,-0.5f, 0.5f,  0,1,0,   0.5f,-0.5f,-0.5f,  0,1,0,   0.5f, 0.5f,-0.5f,  0,1,0,
                0.5f,-0.5f, 0.5f,  0,1,0,   0.5f, 0.5f,-0.5f,  0,1,0,   0.5f, 0.5f, 0.5f,  0,1,0,

                // back
                0.5f,-0.5f,-0.5f,  0,0,1,  -0.5f,-0.5f,-0.5f,  0,0,1,  -0.5f, 0.5f,-0.5f,  0,0,1,
                0.5f,-0.5f,-0.5f,  0,0,1,  -0.5f, 0.5f,-0.5f,  0,0,1,   0.5f, 0.5f,-0.5f,  0,0,1,

                // left
                -0.5f,-0.5f,-0.5f,  1,1,0,  -0.5f,-0.5f, 0.5f,  1,1,0,  -0.5f, 0.5f, 0.5f,  1,1,0,
                -0.5f,-0.5f,-0.5f,  1,1,0,  -0.5f, 0.5f, 0.5f,  1,1,0,  -0.5f, 0.5f,-0.5f,  1,1,0,

                // bottom
                -0.5f,-0.5f,-0.5f,  1,0,1,   0.5f,-0.5f,-0.5f,  1,0,1,   0.5f,-0.5f, 0.5f,  1,0,1,
                -0.5f,-0.5f,-0.5f,  1,0,1,   0.5f,-0.5f, 0.5f,  1,0,1,  -0.5f,-0.5f, 0.5f,  1,0,1,

                // top
                -0.5f, 0.5f, 0.5f,  0,1,1,   0.5f, 0.5f, 0.5f,  0,1,1,   0.5f, 0.5f,-0.5f,  0,1,1,
                -0.5f, 0.5f, 0.5f,  0,1,1,   0.5f, 0.5f,-0.5f,  0,1,1,  -0.5f, 0.5f,-0.5f,  0,1,1,
            };
            
            // Generate VBO
            vertexBufferHandle = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferHandle);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);
            
            // Generate VAO
            vertexArrayHandle = GL.GenVertexArray();
            GL.BindVertexArray(vertexArrayHandle);
           
            // position
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            // color
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));
            GL.EnableVertexAttribArray(1);
            
            GL.BindVertexArray(0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

            // Vertex shader with model, view, projection matrices
            string vertexShaderCode = @"
                #version 330 core
                layout(location = 0) in vec3 aPosition;
                layout(location = 1) in vec3 aColor;

                uniform mat4 uModel;
                uniform mat4 uView;
                uniform mat4 uProj;

                out vec3 vColor;

                void main()
                {
                    vColor = aColor;
                    gl_Position = uProj * uView * uModel * vec4(aPosition, 1.0);
                }
            ";

            string fragmentShaderCode = @"
                #version 330 core
                in vec3 vColor;
                out vec4 FragColor;

                void main()
                {
                    FragColor = vec4(vColor, 1.0);
                }
            ";

            int vertexShaderHandle = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShaderHandle, vertexShaderCode);
            GL.CompileShader(vertexShaderHandle);
            CheckShaderCompile(vertexShaderHandle, "Vertex Shader");

            int fragmentShaderHandle = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragmentShaderHandle, fragmentShaderCode);
            GL.CompileShader(fragmentShaderHandle);
            CheckShaderCompile(fragmentShaderHandle, "Fragment Shader");

            shaderProgramHandle = GL.CreateProgram();
            GL.AttachShader(shaderProgramHandle, vertexShaderHandle);
            GL.AttachShader(shaderProgramHandle, fragmentShaderHandle);
            GL.LinkProgram(shaderProgramHandle);

            GL.DetachShader(shaderProgramHandle, vertexShaderHandle);
            GL.DetachShader(shaderProgramHandle, fragmentShaderHandle);
            GL.DeleteShader(vertexShaderHandle);
            GL.DeleteShader(fragmentShaderHandle);

            // Get uniform locations
            modelLoc = GL.GetUniformLocation(shaderProgramHandle, "uModel");
            viewLoc = GL.GetUniformLocation(shaderProgramHandle, "uView");
            projLoc = GL.GetUniformLocation(shaderProgramHandle, "uProj");
            
            GL.Enable(EnableCap.DepthTest);
            GL.DepthFunc(DepthFunction.Less);
            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Back);
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);
            angle += (float)args.Time;
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.UseProgram(shaderProgramHandle);
            
            // camera view
            Matrix4 view = Matrix4.LookAt(new Vector3(0, 1.5f, 2f), Vector3.Zero, Vector3.UnitY);
           
            // fov
            Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView(
                MathHelper.DegreesToRadians(90f),
                (float)Size.X / Size.Y,
                0.1f, 100f);

            // rotation
            Matrix4 model = Matrix4.CreateRotationY(angle);
            
            GL.UniformMatrix4(viewLoc, false, ref view);
            GL.UniformMatrix4(projLoc, false, ref projection);
            GL.UniformMatrix4(modelLoc, false, ref model);
            
            GL.BindVertexArray(vertexArrayHandle);
            GL.DrawArrays(PrimitiveType.Triangles, 0, vertexCount);
            GL.BindVertexArray(0);
            
            SwapBuffers();        
        }

        protected override void OnUnload()
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.DeleteBuffer(vertexBufferHandle);

            GL.BindVertexArray(0);
            GL.DeleteVertexArray(vertexArrayHandle);

            GL.UseProgram(0);
            GL.DeleteProgram(shaderProgramHandle);

            base.OnUnload();
        }

        private void CheckShaderCompile(int shaderHandle, string shaderName)
        {
            GL.GetShader(shaderHandle, ShaderParameter.CompileStatus, out int success);
            if (success == 0)
            {
                string infoLog = GL.GetShaderInfoLog(shaderHandle);
                Console.WriteLine($"Error compiling {shaderName}: {infoLog}");
            }
        }
    }
}