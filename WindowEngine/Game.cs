using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;
using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
namespace WindowEngine
{
    public class Game : GameWindow
    {
       private int _vertexBufferHandle;
        private int _shaderProgramHandle;
        private int _vertexArrayHandle;

        private int _modelLoc, _viewLoc, _projLoc;
        private int _lightPosLoc, _viewPosLoc, _lightColorLoc, _objectColorLoc;
        
        private float _angle;
        
        // camera speed
        private float _cameraSpeed = 1.5f;

        private Vector2 _lastPos;
        private bool _firstMove = true;
        
        private Camera _camera;
        
        public Game()
            : base(GameWindowSettings.Default, NativeWindowSettings.Default)
        {
            this.Size = new Vector2i(1280, 768);
            this.CenterWindow(this.Size);
        }

        protected override void OnLoad()
        {
            base.OnLoad();
            
            GL.ClearColor(new Color4(0.1f, 0.1f, 0.1f, 1f));        
            
            float[] vertices = {
                // front
                -0.5f,-0.5f, 0.5f,  0,0,1,   
                0.5f,-0.5f, 0.5f,  0,0,1,    
                0.5f, 0.5f, 0.5f,  0,0,1, 
                -0.5f,-0.5f, 0.5f,  0,0,1, 
                0.5f, 0.5f, 0.5f,  0,0,1, 
                -0.5f, 0.5f, 0.5f,  0,0,1, 

                // right
                0.5f,-0.5f, 0.5f,  1,0,0,
                0.5f,-0.5f,-0.5f,  1,0,0,
                0.5f, 0.5f,-0.5f,  1,0,0,
                0.5f,-0.5f, 0.5f,  1,0,0,
                0.5f, 0.5f,-0.5f,  1,0,0,
                0.5f, 0.5f, 0.5f,  1,0,0,

                // back
                0.5f,-0.5f,-0.5f,  0,0,-1,
                -0.5f,-0.5f,-0.5f,  0,0,-1,
                -0.5f, 0.5f,-0.5f,  0,0,-1,
                0.5f,-0.5f,-0.5f,  0,0,-1,
                -0.5f, 0.5f,-0.5f,  0,0,-1,
                0.5f, 0.5f,-0.5f,  0,0,-1,

                // left
                -0.5f,-0.5f,-0.5f,  -1,0,0,
                -0.5f,-0.5f, 0.5f,  -1,0,0,
                -0.5f, 0.5f, 0.5f,  -1,0,0,
                -0.5f,-0.5f,-0.5f,  -1,0,0,
                -0.5f, 0.5f, 0.5f,  -1,0,0,
                -0.5f, 0.5f,-0.5f,  -1,0,0,

                // bottom
                -0.5f,-0.5f,-0.5f,  0,-1,0,
                0.5f,-0.5f,-0.5f,  0,-1,0,
                0.5f,-0.5f, 0.5f,  0,-1,0,
                -0.5f,-0.5f,-0.5f,  0,-1,0,
                0.5f,-0.5f, 0.5f,  0,-1,0,
                -0.5f,-0.5f, 0.5f,  0,-1,0,

                // top
                -0.5f, 0.5f, 0.5f,  0,1,0,
                0.5f, 0.5f, 0.5f,  0,1,0,
                0.5f, 0.5f,-0.5f,  0,1,0,
                -0.5f, 0.5f, 0.5f,  0,1,0,
                0.5f, 0.5f,-0.5f,  0,1,0,
                -0.5f, 0.5f,-0.5f,  0,1,0,
            };
            
            // Generate VBO
            _vertexBufferHandle = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferHandle);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);
            
            // Generate VAO
            _vertexArrayHandle = GL.GenVertexArray();
            GL.BindVertexArray(_vertexArrayHandle);
           
            // position
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            // normal
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));
            GL.EnableVertexAttribArray(1);
            
            GL.BindVertexArray(0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
                
            string vertexShaderCode = File.ReadAllText("Assets/Shaders/phong.vert");
            string fragmentShaderCode = File.ReadAllText("Assets/Shaders/phong.frag");

            int vertexShaderHandle = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShaderHandle, vertexShaderCode);
            GL.CompileShader(vertexShaderHandle);
            CheckShaderCompile(vertexShaderHandle, "Vertex Shader");

            int fragmentShaderHandle = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragmentShaderHandle, fragmentShaderCode);
            GL.CompileShader(fragmentShaderHandle);
            CheckShaderCompile(fragmentShaderHandle, "Fragment Shader");

            _shaderProgramHandle = GL.CreateProgram();
            GL.AttachShader(_shaderProgramHandle, vertexShaderHandle);
            GL.AttachShader(_shaderProgramHandle, fragmentShaderHandle);
            GL.LinkProgram(_shaderProgramHandle);

            GL.DetachShader(_shaderProgramHandle, vertexShaderHandle);
            GL.DetachShader(_shaderProgramHandle, fragmentShaderHandle);
            GL.DeleteShader(vertexShaderHandle);
            GL.DeleteShader(fragmentShaderHandle);

            // Get uniform locations
            _modelLoc = GL.GetUniformLocation(_shaderProgramHandle, "model");
            _viewLoc = GL.GetUniformLocation(_shaderProgramHandle, "view");
            _projLoc = GL.GetUniformLocation(_shaderProgramHandle, "projection");
            
            _lightPosLoc = GL.GetUniformLocation(_shaderProgramHandle, "lightPos");
            _viewPosLoc = GL.GetUniformLocation(_shaderProgramHandle, "viewPos");
            _lightColorLoc = GL.GetUniformLocation(_shaderProgramHandle, "lightColor");
            _objectColorLoc = GL.GetUniformLocation(_shaderProgramHandle, "objectColor");
            
            GL.Enable(EnableCap.DepthTest);
            GL.DepthFunc(DepthFunction.Less);
            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Back);

            _camera = new Camera(Vector3.UnitZ * 3, Size.X / (float)Size.Y);
                
            CursorState = CursorState.Grabbed; 
        } 

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);
            
            _angle += (float)e.Time;
            
            const float sensitivity = 0.2f;
            
            if (!IsFocused) // check to see if the window is focused
            {
                return;
            }

            KeyboardState input = KeyboardState;
            
            if (input.IsKeyDown(Keys.Escape))
            {
                Close();
            }
            
            if (input.IsKeyDown(Keys.W))
            {
                _camera.Position += _camera.Front * _cameraSpeed * (float)e.Time; // Forward
            }

            if (input.IsKeyDown(Keys.S))
            {
                _camera.Position -= _camera.Front * _cameraSpeed * (float)e.Time; // Backwards
            }
            if (input.IsKeyDown(Keys.A))
            {
                _camera.Position -= _camera.Right * _cameraSpeed * (float)e.Time; // Left
            }
            if (input.IsKeyDown(Keys.D))
            {
                _camera.Position += _camera.Right * _cameraSpeed * (float)e.Time; // Right
            }
            if (input.IsKeyDown(Keys.Space))
            {
                _camera.Position += _camera.Up * _cameraSpeed * (float)e.Time; // Up
            }
            if (input.IsKeyDown(Keys.LeftShift))
            {
                _camera.Position -= _camera.Up * _cameraSpeed * (float)e.Time; // Down
            }
            
            // Get the mouse state
            var mouse = MouseState;

            if (_firstMove) // This bool variable is initially set to true.
            {
                _lastPos = new Vector2(mouse.X, mouse.Y);
                _firstMove = false;
            }
            else
            {
                // Calculate the offset of the mouse position
                var deltaX = mouse.X - _lastPos.X;
                var deltaY = mouse.Y - _lastPos.Y;
                _lastPos = new Vector2(mouse.X, mouse.Y);

                // Apply the camera pitch and yaw (we clamp the pitch in the camera class)
                _camera.Yaw += deltaX * sensitivity;
                _camera.Pitch -= deltaY * sensitivity; // Reversed since y-coordinates range from bottom to top
            }
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.UseProgram(_shaderProgramHandle);
            
            // camera view
            Matrix4 view = _camera.GetViewMatrix();
           
            // fov
            Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView(
                MathHelper.DegreesToRadians(80f),
                (float)Size.X / Size.Y,
                0.1f, 100f);

            // rotation
            Matrix4 model = Matrix4.CreateRotationY(_angle * 0.7f);

            Vector3 camPos = _camera.Position;
            Vector3 lightPos = new Vector3(1.2f, 1.0f, 2.0f);

            GL.UniformMatrix4(_viewLoc, false, ref view);
            GL.UniformMatrix4(_projLoc, false, ref projection);
            GL.UniformMatrix4(_modelLoc, false, ref model);

            GL.Uniform3(_lightPosLoc, ref lightPos);
            GL.Uniform3(_viewPosLoc, ref camPos);
            
            Vector3 lightColor = Vector3.One;
            Vector3 objectColor = new Vector3(0.6f, 0.7f, 0.1f);
            GL.Uniform3(_lightColorLoc, ref lightColor);
            GL.Uniform3(_objectColorLoc, ref objectColor);
            
            GL.BindVertexArray(_vertexArrayHandle);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 36);
            GL.BindVertexArray(0);
            
            SwapBuffers();        
        }

        protected override void OnUnload()
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.DeleteBuffer(_vertexBufferHandle);

            GL.BindVertexArray(0);
            GL.DeleteVertexArray(_vertexArrayHandle);

            GL.UseProgram(0);
            GL.DeleteProgram(_shaderProgramHandle);

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
        
        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);

            GL.Viewport(0, 0, Size.X, Size.Y);
        }
    }
}