// File: Program.cs
//
// Fix explained: some OpenTK versions expose
// Matrix4.CreateOrthographicOffCenter(float left, float right, float bottom, float top, float zNear, float zFar)
// but older/newer API names may differ (e.g., near, far). Using *named* args can break across versions.
// We switch to *positional* args to be version-agnostic: CreateOrthographicOffCenter(0, 800, 0, 600, -1, 1).

using OpenTK.Graphics.OpenGL4;                       // OpenGL API
using OpenTK.Windowing.Common;                       // Frame events (OnLoad/OnUpdate/OnRender)
using OpenTK.Windowing.Desktop;                      // GameWindow/NativeWindowSettings
using OpenTK.Windowing.GraphicsLibraryFramework;     // Keyboard state
using OpenTK.Mathematics;                            // Matrix4, Vector types
using System;
using System.IO;
using ImageSharp = SixLabors.ImageSharp.Image;       // Alias for brevity
using SixLabors.ImageSharp.PixelFormats;             // Rgba32 pixel type

namespace WindowEngine
{
    public class SpriteAnimationGame : GameWindow
    {
        private Character _character;                 // Handles animation state + UV selection
        private int _shaderProgram;                   // Linked GLSL program
        private int _vao, _vbo;                       // Geometry
        private int _bgVao, _bgVbo;
        private int _poleVao, _poleVbo;
        private int _texture;                         // Sprite sheet
        private int _bgTexture;                       // background sprite
        private int _poleTexture;                     // pole sprite
        
        private int _modelLoc;

        public SpriteAnimationGame()
            : base(
                new GameWindowSettings(),
                new NativeWindowSettings { Size = (800, 600), Title = "Sprite Animation" })
        { }

        protected override void OnLoad()
        {
            base.OnLoad();

            GL.ClearColor(0f, 0f, 0f, 0f);            // Transparent background (A=0)
            GL.Enable(EnableCap.Blend);               // Enable alpha blending
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            _shaderProgram = CreateShaderProgram();   // Compile + link
            _texture = LoadTexture("Assets/Sprites/sprite.png"); // Upload sprite sheet
            
            _bgTexture = LoadTexture("Assets/Sprites/bg.png");
            _poleTexture = LoadTexture("Assets/Sprites/pole.png");

            float w = 120f, h = 85f;
            float[] vertices =
            {
                -w, -h, 0f, 0f,
                w, -h, 1f, 0f,
                w,  h, 1f, 1f,
                -w,  h, 0f, 1f
            };

            {
                _vao = GL.GenVertexArray();
                GL.BindVertexArray(_vao);

                _vbo = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
                GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

                GL.EnableVertexAttribArray(0);
                GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), 0);

                GL.EnableVertexAttribArray(1);
                GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), 2 * sizeof(float));
            }
            

            float[] bgVerts = {
                -400f, -300f, 0f, 0f,
                400f, -300f, 1f, 0f,
                400f,  300f, 1f, 1f,
                -400f,  300f, 0f, 1f
            };

            {
                _bgVao = GL.GenVertexArray();
                GL.BindVertexArray(_bgVao);
                
                _bgVbo = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ArrayBuffer, _bgVbo);
                GL.BufferData(BufferTarget.ArrayBuffer, bgVerts.Length * sizeof(float), bgVerts, BufferUsageHint.StaticDraw);
                
                GL.EnableVertexAttribArray(0);
                GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), 0);
                
                GL.EnableVertexAttribArray(1);
                GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), 2 * sizeof(float));
            }

            float poleW = 45f, poleH = 292f;
            float[] poleVerts = {
                -poleW, -poleH, 0f, 0f,
                poleW, -poleH, 1f, 0f,
                poleW,  poleH, 1f, 1f,
                -poleW,  poleH, 0f, 1f
            };

            {
                _poleVao = GL.GenVertexArray();
                GL.BindVertexArray(_poleVao);
                
                _poleVbo = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ArrayBuffer, _poleVbo);
                GL.BufferData(BufferTarget.ArrayBuffer, poleVerts.Length * sizeof(float), poleVerts, BufferUsageHint.StaticDraw);
                
                GL.EnableVertexAttribArray(0); 
                GL.VertexAttribPointer(0,2,VertexAttribPointerType.Float,false,4*sizeof(float),0);
                
                GL.EnableVertexAttribArray(1); 
                GL.VertexAttribPointer(1,2,VertexAttribPointerType.Float,false,4*sizeof(float),2*sizeof(float));
            }
            
            GL.UseProgram(_shaderProgram);
            
            _modelLoc = GL.GetUniformLocation(_shaderProgram, "model");

            int texLoc = GL.GetUniformLocation(_shaderProgram, "uTexture");
            GL.Uniform1(texLoc, 0);

            int projLoc = GL.GetUniformLocation(_shaderProgram, "projection");
            Matrix4 ortho = Matrix4.CreateOrthographicOffCenter(0, 800, 0, 600, -1, 1);
            GL.UniformMatrix4(projLoc, false, ref ortho);

            int modelLoc = GL.GetUniformLocation(_shaderProgram, "model");
            Matrix4 model = Matrix4.CreateTranslation(400, 300, 0);
            GL.UniformMatrix4(modelLoc, false, ref model);

            _character = new Character(_shaderProgram); // Initializes idle frame uniforms
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            var keyboard = KeyboardState;

            bool left = keyboard.IsKeyDown(Keys.Left);
            bool right = keyboard.IsKeyDown(Keys.Right);

            // Edge detection for single-press actions
            bool upPressedEdge = keyboard.IsKeyDown(Keys.Up);
            bool punchPressedEdge = keyboard.IsKeyDown(Keys.Space);
            
            // Give character input
            _character.HandleInput((float)e.Time, left, right, upPressedEdge, punchPressedEdge);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
            GL.Clear(ClearBufferMask.ColorBufferBit);

            GL.BindTexture(TextureTarget.Texture2D, (_bgTexture));
            GL.BindVertexArray(_bgVao);
            
            // whole texture
            int off = GL.GetUniformLocation(_shaderProgram, "uOffset");
            int sz = GL.GetUniformLocation(_shaderProgram, "uSize");
            GL.Uniform2(off, 0f, 0f);
            GL.Uniform2(sz, 1f, 1f);
            
            Matrix4 bgModel = Matrix4.CreateTranslation(400f, 300f, 0f);
            GL.UniformMatrix4(_modelLoc, false, ref bgModel);
            
            GL.DrawArrays(PrimitiveType.TriangleFan, 0, 4);
            
            GL.BindTexture(TextureTarget.Texture2D, _texture);
            GL.BindVertexArray(_vao);

            var pos = _character.Position;
            float scaleX = _character.Facing == Direction.Left ? -1f : 1f;
            Matrix4 model = Matrix4.CreateTranslation(-0f, -0f, 0f) *
                            Matrix4.CreateScale(scaleX, 1f, 1f) *
                            Matrix4.CreateTranslation(pos.X, pos.Y, 0f);

            GL.UseProgram(_shaderProgram);
            GL.UniformMatrix4(_modelLoc, false, ref model);

            _character.Render();
            
            GL.BindTexture(TextureTarget.Texture2D, (_poleTexture));
            GL.BindVertexArray(_poleVao);
            
            // whole texture
            off = GL.GetUniformLocation(_shaderProgram, "uOffset");
            sz = GL.GetUniformLocation(_shaderProgram, "uSize");
            GL.Uniform2(off, 0f, 0f);
            GL.Uniform2(sz, 1f, 1f);
            
            Matrix4 poleModel = Matrix4.CreateTranslation(82f, 292f, 0f);
            GL.UniformMatrix4(_modelLoc, false, ref poleModel);
            
            GL.DrawArrays(PrimitiveType.TriangleFan, 0, 4);

            SwapBuffers();
        }

        protected override void OnUnload()
        {
            // Free GPU resources
            GL.DeleteProgram(_shaderProgram);
            GL.DeleteTexture(_texture);
            GL.DeleteTexture(_poleTexture);
            GL.DeleteTexture(_bgTexture);
            GL.DeleteBuffer(_vbo);
            GL.DeleteVertexArray(_vao);
            GL.DeleteBuffer(_poleVbo);
            GL.DeleteVertexArray(_poleVao);
            GL.DeleteBuffer(_bgVbo);
            GL.DeleteVertexArray(_bgVao);
            base.OnUnload();
        }

        private int CreateShaderProgram()
        {
            // Vertex Shader: transforms positions, flips V in UVs (image origin vs GL origin)
            string vs = @"
                #version 330 core
                layout(location = 0) in vec2 aPosition;
                layout(location = 1) in vec2 aTexCoord;
                out vec2 vTexCoord;
                uniform mat4 projection;
                uniform mat4 model;
                void main() {
                    gl_Position = projection * model * vec4(aPosition, 0.0, 1.0);
                    vTexCoord = vec2(aTexCoord.x, 1.0 - aTexCoord.y); // flip V so PNGs read intuitively
                }";

            // Fragment Shader: samples sub-rect of the sheet using uOffset/uSize
            string fs = @"
                #version 330 core
                in vec2 vTexCoord;
                out vec4 color;
                uniform sampler2D uTexture; // bound to texture unit 0
                uniform vec2 uOffset;       // normalized UV start (0..1)
                uniform vec2 uSize;         // normalized UV size  (0..1)
                void main() {
                    vec2 uv = uOffset + vTexCoord * uSize;
                    color = texture(uTexture, uv);
                }";

            int v = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(v, vs);
            GL.CompileShader(v);
            CheckShaderCompile(v, "VERTEX");

            int f = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(f, fs);
            GL.CompileShader(f);
            CheckShaderCompile(f, "FRAGMENT");

            int p = GL.CreateProgram();
            GL.AttachShader(p, v);
            GL.AttachShader(p, f);
            GL.LinkProgram(p);
            CheckProgramLink(p);

            GL.DetachShader(p, v);
            GL.DetachShader(p, f);
            GL.DeleteShader(v);
            GL.DeleteShader(f);

            return p;
        }

        private static void CheckShaderCompile(int shader, string stage)
        {
            GL.GetShader(shader, ShaderParameter.CompileStatus, out int ok);
            if (ok == 0)
                throw new Exception($"{stage} SHADER COMPILE ERROR:\n{GL.GetShaderInfoLog(shader)}");
        }

        private static void CheckProgramLink(int program)
        {
            GL.GetProgram(program, GetProgramParameterName.LinkStatus, out int ok);
            if (ok == 0)
                throw new Exception($"PROGRAM LINK ERROR:\n{GL.GetProgramInfoLog(program)}");
        }

        private int LoadTexture(string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException($"Texture not found: {path}", path);

            using var img = ImageSharp.Load<Rgba32>(path); // decode to RGBA8

            int tex = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, tex);

            // Copy raw pixels to managed buffer then upload
            var pixels = new byte[4 * img.Width * img.Height];
            img.CopyPixelDataTo(pixels);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba,
                          img.Width, img.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, pixels);

            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

            // Nearest: prevents bleeding between adjacent frames on the atlas
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);

            // Clamp: avoid wrap artifacts at frame borders
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);

            return tex;
        }
    }

    // --- Direction input abstraction -----------------------------------------------------------
    public enum Direction { None, Right, Left, Up }

    // --- Entry point ---------------------------------------------------------------------------
    internal class Program
    {
        private static void Main()
        {
            using var game = new SpriteAnimationGame(); // Ensures Dispose/OnUnload is called
            game.Run();                                  // Game loop: Load -> (Update/Render)* -> Unload
        }
    }
}
