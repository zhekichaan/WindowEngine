using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace WindowEngine
{
    public class Game : GameWindow
    {
        private int _vaoLamp;

        private Shader _lampShader;
        private Shader _lightingShader;

        private Camera _camera;

        private bool _firstMove = true;

        private Vector2 _lastPos;
        
        private Mesh _ground;

        private Mesh _tree;
        private Mesh _tree2;
        
        private Mesh _house;
        private Mesh _fence;
        private Mesh _lamp;
        
        private bool _isLightOn = true;

        private readonly Vector3 _lightZoneMin = new Vector3(-5f, -1f, -12f);
        private readonly Vector3 _lightZoneMax = new Vector3(1f, 3f, -6f);
        
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
            
            _tree = new Mesh("tree", "Assets/Models/tree01.fbx", _lightingShader, Texture.LoadFromFile("Assets/Textures/tree01.png"), _camera);
            _ground = new Mesh("ground", "Assets/Models/terrain.fbx", _lightingShader, Texture.LoadFromFile("Assets/Textures/dirt.png"), _camera );
            _tree2 = new Mesh("tree2", "Assets/Models/tree12.fbx", _lightingShader, Texture.LoadFromFile("Assets/Textures/tree12.png"), _camera);
            _house = new Mesh("house", "Assets/Models/house.fbx", _lightingShader, Texture.LoadFromFile("Assets/Textures/house.png"), _camera);
            _fence = new Mesh("fence", "Assets/Models/fence.fbx", _lightingShader, Texture.LoadFromFile("Assets/Textures/metal.png"), _camera);
            _lamp = new Mesh("lamp", "Assets/Models/lamp.fbx", _lightingShader, Texture.LoadFromFile("Assets/Textures/lamp.png"), _camera);
            
            CursorState = CursorState.Grabbed;

            _worldObjects = new List<WorldObject>();
            _worldObjects.Add(new WorldObject(_ground, new Vector3(0, 0, 0), new Vector3(1f), 0));             
            _worldObjects.Add(new WorldObject(_house, new Vector3(0, -0.9f, -10), new Vector3(0.01f), 0));             
            _worldObjects.Add(new WorldObject(_tree, new Vector3(7, 0, 7), new Vector3(0.01f), 0));              
            _worldObjects.Add(new WorldObject(_tree2, new Vector3(-10, 0, -6), new Vector3(0.01f), 0));      
            _worldObjects.Add(new WorldObject(_lamp, new Vector3(-2f, 3f, -8f), new Vector3(0.15f), 0));      
            
            float gap = -12f;
            for (int i = 0; i < 8; i++)
            {
                _worldObjects.Add(new WorldObject(_fence, new Vector3(gap, 1, -16), new Vector3(1f), 0));
                gap += 3f;
            }
            
            gap = -14.5f;
            for (var i = 0; i < 8; i++)
            {
                _worldObjects.Add(new WorldObject(_fence, new Vector3(10.5f, 1, gap), new Vector3(1f), float.DegreesToRadians(90f)));
                gap += 3f;
            }
            
            gap = -12f;
            for (var i = 0; i < 8; i++)
            {
                _worldObjects.Add(new WorldObject(_fence, new Vector3(gap, 1, 8.2f), new Vector3(1f), 0));
                gap += 3f;
            }
            
            gap = -14.5f;
            for (var i = 0; i < 8; i++)
            {
                _worldObjects.Add(new WorldObject(_fence, new Vector3(-13.5f, 1, gap), new Vector3(1f), float.DegreesToRadians(90f)));
                gap += 3f;
            }
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            
            Vector3 diffuseColor = _isLightOn ? new Vector3(0.5f) : Vector3.Zero;
            
            foreach(var obj in _worldObjects)
            {
                if (obj.Mesh.Name.Equals("lamp"))
                {
                    Vector3 lampColor = _isLightOn ? new Vector3(1.0f, 1.0f, 0.6f) : new Vector3(0.2f);
                    _lampShader.SetVector3("lightColor", lampColor);
                    
                    obj.Draw(diffuseColor, _lampShader);
                }
                else
                {
                    obj.Draw(diffuseColor, _lightingShader);
                }
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
                IsPlayerInZone(_camera.Position, _lightZoneMin, _lightZoneMax))
            {
                _isLightOn = !_isLightOn;
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