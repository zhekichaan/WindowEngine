using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace WindowEngine
{
    public class Game : GameWindow
    {
        private float[] _groundVertices = {
            // positions            normals         texcoords
            -50f, 0f, -50f,         0f, 1f, 0f,     0.0f, 50f,
            50f, 0f, -50f,         0f, 1f, 0f,     50f, 50f,
            50f, 0f,  50f,         0f, 1f, 0f,     50f, 0.0f,

            50f, 0f,  50f,         0f, 1f, 0f,     50f, 0.0f,
            -50f, 0f,  50f,         0f, 1f, 0f,     0.0f, 0.0f,
            -50f, 0f, -50f,         0f, 1f, 0f,     0.0f, 50f
        };
        
        private readonly float[] _vertices =
        {
            // Positions          Normals              Texture coords
            -0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  0.0f, 0.0f,
             0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  1.0f, 0.0f,
             0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  1.0f, 1.0f,
             0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  1.0f, 1.0f,
            -0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  0.0f, 1.0f,
            -0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  0.0f, 0.0f,

            -0.5f, -0.5f,  0.5f,  0.0f,  0.0f,  1.0f,  0.0f, 0.0f,
             0.5f, -0.5f,  0.5f,  0.0f,  0.0f,  1.0f,  1.0f, 0.0f,
             0.5f,  0.5f,  0.5f,  0.0f,  0.0f,  1.0f,  1.0f, 1.0f,
             0.5f,  0.5f,  0.5f,  0.0f,  0.0f,  1.0f,  1.0f, 1.0f,
            -0.5f,  0.5f,  0.5f,  0.0f,  0.0f,  1.0f,  0.0f, 1.0f,
            -0.5f, -0.5f,  0.5f,  0.0f,  0.0f,  1.0f,  0.0f, 0.0f,

            -0.5f,  0.5f,  0.5f, -1.0f,  0.0f,  0.0f,  1.0f, 0.0f,
            -0.5f,  0.5f, -0.5f, -1.0f,  0.0f,  0.0f,  1.0f, 1.0f,
            -0.5f, -0.5f, -0.5f, -1.0f,  0.0f,  0.0f,  0.0f, 1.0f,
            -0.5f, -0.5f, -0.5f, -1.0f,  0.0f,  0.0f,  0.0f, 1.0f,
            -0.5f, -0.5f,  0.5f, -1.0f,  0.0f,  0.0f,  0.0f, 0.0f,
            -0.5f,  0.5f,  0.5f, -1.0f,  0.0f,  0.0f,  1.0f, 0.0f,

             0.5f,  0.5f,  0.5f,  1.0f,  0.0f,  0.0f,  1.0f, 0.0f,
             0.5f,  0.5f, -0.5f,  1.0f,  0.0f,  0.0f,  1.0f, 1.0f,
             0.5f, -0.5f, -0.5f,  1.0f,  0.0f,  0.0f,  0.0f, 1.0f,
             0.5f, -0.5f, -0.5f,  1.0f,  0.0f,  0.0f,  0.0f, 1.0f,
             0.5f, -0.5f,  0.5f,  1.0f,  0.0f,  0.0f,  0.0f, 0.0f,
             0.5f,  0.5f,  0.5f,  1.0f,  0.0f,  0.0f,  1.0f, 0.0f,

            -0.5f, -0.5f, -0.5f,  0.0f, -1.0f,  0.0f,  0.0f, 1.0f,
             0.5f, -0.5f, -0.5f,  0.0f, -1.0f,  0.0f,  1.0f, 1.0f,
             0.5f, -0.5f,  0.5f,  0.0f, -1.0f,  0.0f,  1.0f, 0.0f,
             0.5f, -0.5f,  0.5f,  0.0f, -1.0f,  0.0f,  1.0f, 0.0f,
            -0.5f, -0.5f,  0.5f,  0.0f, -1.0f,  0.0f,  0.0f, 0.0f,
            -0.5f, -0.5f, -0.5f,  0.0f, -1.0f,  0.0f,  0.0f, 1.0f,

            -0.5f,  0.5f, -0.5f,  0.0f,  1.0f,  0.0f,  0.0f, 1.0f,
             0.5f,  0.5f, -0.5f,  0.0f,  1.0f,  0.0f,  1.0f, 1.0f,
             0.5f,  0.5f,  0.5f,  0.0f,  1.0f,  0.0f,  1.0f, 0.0f,
             0.5f,  0.5f,  0.5f,  0.0f,  1.0f,  0.0f,  1.0f, 0.0f,
            -0.5f,  0.5f,  0.5f,  0.0f,  1.0f,  0.0f,  0.0f, 0.0f,
            -0.5f,  0.5f, -0.5f,  0.0f,  1.0f,  0.0f,  0.0f, 1.0f
        };

        private readonly Vector3 _lightPos = new Vector3(-2f, 3f, -18f);

        private int _vertexBufferObject;

        private int _vaoModel;

        private int _vaoLamp;

        private Shader _lampShader;
        private Shader _lightingShader;

        private Camera _camera;

        private bool _firstMove = true;

        private Vector2 _lastPos;
        
        private Mesh _ground;
        private Mesh _box1;

        private Mesh _tree;
        private Mesh _tree2;
        
        private Mesh _house;
        
        private bool isLightOn = true;

        private Vector3 lightZoneMin = new Vector3(-5f, -1f, -22f);
        private Vector3 lightZoneMax = new Vector3(1f, 3f, -15f);
        
        List<WorldObject> _worldObjects;
        
        public Game()
            : base(GameWindowSettings.Default, NativeWindowSettings.Default)
        {
            this.Size = new Vector2i(1280, 768);
            this.CenterWindow(this.Size);
        }

       protected override void OnLoad()
        {
            base.OnLoad();

            GL.ClearColor(0.01f, 0.01f, 0.03f, 0.1f);

            GL.Enable(EnableCap.DepthTest);
            
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            _lightingShader = new Shader("Assets/Shaders/shader.vert", "Assets/Shaders/lighting.frag");
            _lampShader = new Shader("Assets/Shaders/shader.vert", "Assets/Shaders/shader.frag");
            
            _camera = new Camera(Vector3.UnitY * 1.8f, Size.X / (float)Size.Y);
            
            _tree = new Mesh("Assets/Models/tree01.fbx", _lightingShader, Texture.LoadFromFile("Assets/Textures/tree01.png"), _camera);
            _ground = new Mesh("Assets/Models/terrain.fbx", _lightingShader, Texture.LoadFromFile("Assets/Textures/dirt.png"), _camera );
            _tree2 = new Mesh("Assets/Models/tree12.fbx", _lightingShader, Texture.LoadFromFile("Assets/Textures/tree12.png"), _camera);
            _house = new Mesh("Assets/Models/house.fbx", _lightingShader, Texture.LoadFromFile("Assets/Textures/house.png"), _camera);
            
            CursorState = CursorState.Grabbed;

            _worldObjects = new List<WorldObject>();
            _worldObjects.Add(new WorldObject(_ground, new Vector3(0, 0, 0), new Vector3(50, 1, 50)));
            _worldObjects.Add(new WorldObject(_house, new Vector3(0, -0.9f, -20), new Vector3(0.01f)));             
            _worldObjects.Add(new WorldObject(_tree, new Vector3(5, 0, 5), new Vector3(0.01f)));              
            _worldObjects.Add(new WorldObject(_tree2, new Vector3(-5, 0, -5), new Vector3(0.01f)));      
            
            {
                _vaoLamp = GL.GenVertexArray();
                GL.BindVertexArray(_vaoLamp);

                var positionLocation = _lampShader.GetAttribLocation("aPos");
                GL.EnableVertexAttribArray(positionLocation);
                GL.VertexAttribPointer(positionLocation, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 0);
            }
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            
            Vector3 diffuseColor = isLightOn ? new Vector3(0.5f) : Vector3.Zero;
            
            foreach(var obj in _worldObjects)
            {
                obj.Draw();
                
                _lightingShader.Use();

                _lightingShader.SetMatrix4("model", obj.Mesh.Transform);
                _lightingShader.SetMatrix4("view", _camera.GetViewMatrix());
                _lightingShader.SetMatrix4("projection", _camera.GetProjectionMatrix());
        
                _lightingShader.SetInt("material.diffuse", 0);

                _lightingShader.SetVector3("light.position", _lightPos);
                _lightingShader.SetVector3("light.ambient", new Vector3(0.2f));
                _lightingShader.SetVector3("light.diffuse", diffuseColor);
                
                GL.DrawArrays(OpenTK.Graphics.OpenGL4.PrimitiveType.Triangles, 0, obj.Mesh.vericesLength / 8);
            }
            
            SwapBuffers();
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            if (!IsFocused)
            {
                return;
            }

            var input = KeyboardState;

            if (input.IsKeyDown(Keys.Escape))
            {
                Close();
            }
            
            if (input.IsKeyPressed(Keys.E) &&
                IsPlayerInZone(_camera.Position, lightZoneMin, lightZoneMax))
            {
                isLightOn = !isLightOn;
            }

            float cameraSpeed = 2f;
            const float sensitivity = 0.2f;
            
            float yaw = MathHelper.DegreesToRadians(_camera.Yaw);

            Vector3 forward = new Vector3(MathF.Cos(yaw), 0f, MathF.Sin(yaw));
            forward = Vector3.Normalize(forward);

            if (input.IsKeyDown(Keys.LeftShift))
            {
                cameraSpeed = 4f;
            }

            if (input.IsKeyDown(Keys.W))
            {
                _camera.Position += forward * cameraSpeed * (float)e.Time; // Forward
            }
            if (input.IsKeyDown(Keys.S))
            {
                _camera.Position -= forward * cameraSpeed * (float)e.Time; // Backwards
            }
            if (input.IsKeyDown(Keys.A))
            {
                _camera.Position -= _camera.Right * cameraSpeed * (float)e.Time; // Left
            }
            if (input.IsKeyDown(Keys.D))
            {
                _camera.Position += _camera.Right * cameraSpeed * (float)e.Time; // Right
            }

            var mouse = MouseState;

            if (_firstMove)
            {
                _lastPos = new Vector2(mouse.X, mouse.Y);
                _firstMove = false;
            }
            else
            {
                var deltaX = mouse.X - _lastPos.X;
                var deltaY = mouse.Y - _lastPos.Y;
                _lastPos = new Vector2(mouse.X, mouse.Y);

                _camera.Yaw += deltaX * sensitivity;
                _camera.Pitch -= deltaY * sensitivity;
            }
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);
            
            _camera.Fov -= e.OffsetY;
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            GL.Viewport(0, 0, e.Width, e.Height);
            base.OnResize(e);
        }
        
        private bool IsPlayerInZone(Vector3 playerPos, Vector3 min, Vector3 max)
        {
            return playerPos.X >= min.X && playerPos.X <= max.X &&
                   playerPos.Y >= min.Y && playerPos.Y <= max.Y &&
                   playerPos.Z >= min.Z && playerPos.Z <= max.Z;
        }
    }
}