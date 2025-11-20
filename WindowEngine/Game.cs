using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace WindowEngine
{
    public class Game : GameWindow
    {
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
        
        private Mesh _skeleton;
        private Mesh _door;
        
        private bool _isLightOn = true;
        private Door _houseDoor;

        private readonly Vector3 _lightZoneMin = new Vector3(-5f, -1f, -11f);
        private readonly Vector3 _lightZoneMax = new Vector3(1f, 3f, -5f);
        
        List<WorldObject> _worldObjects;
        
        // Player collision settings
        private readonly Vector3 _playerSize = new Vector3(0.6f, 1.8f, 0.6f); // Width, Height, Depth
        
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
            _fence = new Mesh("Assets/Models/fence.fbx", _lightingShader, Texture.LoadFromFile("Assets/Textures/metal.png"), _camera);
            _lamp = new Mesh("Assets/Models/lamp.fbx", _lampShader, Texture.LoadFromFile("Assets/Textures/lamp.png"), _camera);
            
            _skeleton = new Mesh("Assets/Models/skeleton.fbx", _lightingShader, Texture.LoadFromFile("Assets/Textures/skeleton.png"), _camera);
            _door = new Mesh("Assets/Models/door.fbx", _lightingShader, Texture.LoadFromFile("Assets/Textures/door.png"), _camera);
            Console.WriteLine("Door mesh loaded successfully");
            
            CursorState = CursorState.Grabbed;

            
            _worldObjects = new List<WorldObject>();
            
            _houseDoor = new Door(
                _door, 
                new Vector3(0f, 1.1f, -5f),      // Adjust Y height
                new Vector3(1f),               // Adjust scale
                MathHelper.DegreesToRadians(0f), // Adjust initial rotation (try 0, 90, 180, 270)
                new Vector3(1f, 2f, 0.1f)        // Collision size
            );
            _worldObjects.Add(_houseDoor);
            
            _worldObjects.Add(new WorldObject(_ground, new Vector3(0, 0, 0), new Vector3(1f), 0));             
            List<CollisionBoxData> houseCollisionBoxes = new List<CollisionBoxData>
            {
                // Front LEFT wall (from left corner to door)
                new CollisionBoxData(
                    new Vector3(3.4f, 3f, 0.3f),           // 2 units wide (half of front)
                    new Vector3(-2.4f, 1.9f, 5f)           // Positioned left of door
                ),
    
                // Front RIGHT wall (from door to right corner)
                new CollisionBoxData(
                    new Vector3(3.4f, 3f, 0.3f),           // 2 units wide (half of front)
                    new Vector3(2.4f, 1.9f, 5f)            // Positioned right of door
                ),
    
                // LEFT side wall
                new CollisionBoxData(
                    new Vector3(0.3f, 3f, 10f),          // 10 units deep
                    new Vector3(-4f, 1.9f, 0f)           // At x = -4
                ),
    
                // RIGHT side wall
                new CollisionBoxData(
                    new Vector3(0.3f, 3f, 10f),          // 10 units deep
                    new Vector3(4f, 1.9f, 0f)            // At x = +4
                ),
    
                // BACK wall
                new CollisionBoxData(
                    new Vector3(8f, 3f, 0.3f),           // Full 8 units wide
                    new Vector3(0f, 1.9f, -5f)           // At z = -15 (offset -5 from center at -10)
                ),
                
                // Inside walls
                new CollisionBoxData(
                    new Vector3(0.1f, 3f, 3f), // 3 wide, .1 deep
                    new Vector3(1f, 1.9f, 1.4f) 
                ),
                
                new CollisionBoxData(
                    new Vector3(2.6f, 3f, 0.1f), // 2.6 wide, .1 deep
                    new Vector3(2f, 1.9f, 0f) 
                ),
                
                new CollisionBoxData(
                    new Vector3(2.8f, 3f, 0.1f), // 2.8 wide, .1 deep
                    new Vector3(-2f, 1.9f, 0f) 
                ),
                
                new CollisionBoxData(
                    new Vector3(2.3f, 3f, 0.1f), // 2.3 wide, .1 deep
                    new Vector3(2.3f, 1.9f, -3f) 
                ),
                
                new CollisionBoxData(
                    new Vector3(0.1f, 3f, 1f), // 1 wide, .1 deep
                    new Vector3(1f, 1.9f, -0.5f) 
                ),
                
                new CollisionBoxData(
                    new Vector3(0.1f, 3f, 1.5f), // 1.5 wide, .1 deep
                    new Vector3(1f, 1.9f, -2.7f) 
                ),
                
                new CollisionBoxData(
                    new Vector3(0.1f, 3f, 0.5f), // 0.5 wide, .1 deep
                    new Vector3(1f, 1.9f, -4.2f) 
                ),
            };

            _worldObjects.Add(new WorldObject(_house, new Vector3(0, -0.9f, -10), new Vector3(0.01f), 0, houseCollisionBoxes));
            _worldObjects.Add(new WorldObject(_tree, new Vector3(7, 0, 7), new Vector3(0.01f), 0, new Vector3(0.5f, 3f, 0.5f)));     
            _worldObjects.Add(new WorldObject(_tree2, new Vector3(-10, 0, -6), new Vector3(0.01f), 0, new Vector3(0.5f, 3f, 0.5f)));      
            _worldObjects.Add(new WorldObject(_lamp, new Vector3(-2f, 3f, -8f), new Vector3(0.15f), 0));      
            _worldObjects.Add(new WorldObject(_skeleton, new Vector3(3.3f, 1f, -14f), new Vector3(0.4f), float.DegreesToRadians(-90)));      
            
            
            
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
            
            Vector3 lampColor = _isLightOn ? new Vector3(1.0f, 1.0f, 0.6f) : new Vector3(0.2f);
            _lampShader.SetVector3("lightColor", lampColor);
            
            foreach(var obj in _worldObjects)
            {
                obj.Draw(diffuseColor);
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

            Vector3 oldPosition = _camera.Position;
            Vector3 newPosition = oldPosition;

            // Movement with collision detection
            if (input.IsKeyDown(Keys.W))
            {
                newPosition += forward * cameraSpeed * (float)e.Time;
            }
            if (input.IsKeyDown(Keys.S))
            {
                newPosition -= forward * cameraSpeed * (float)e.Time;
            }
            if (input.IsKeyDown(Keys.A))
            {
                newPosition -= _camera.Right * cameraSpeed * (float)e.Time;
            }
            if (input.IsKeyDown(Keys.D))
            {
                newPosition += _camera.Right * cameraSpeed * (float)e.Time;
            }
            
            if (input.IsKeyPressed(Keys.P))  // Press P to print position
            {
                Console.WriteLine($"Camera Position: {_camera.Position}");
            }

            // Check collision before applying movement
            if (!CheckPlayerCollision(newPosition))
            {
                _camera.Position = newPosition;
            }
            else
            {
                // Try sliding along walls - check X and Z separately
                Vector3 tryX = new Vector3(newPosition.X, oldPosition.Y, oldPosition.Z);
                Vector3 tryZ = new Vector3(oldPosition.X, oldPosition.Y, newPosition.Z);
                
                if (!CheckPlayerCollision(tryX))
                {
                    _camera.Position = tryX;
                }
                else if (!CheckPlayerCollision(tryZ))
                {
                    _camera.Position = tryZ;
                }
                // If both collide, stay at old position
            }


            var mouse = MouseState;

            if (mouse.IsButtonPressed(MouseButton.Right))
            {
                Door? lookedAtDoor = GetLookedAtDoor(2f); // 2 units max distance
                if (lookedAtDoor != null)
                {
                    lookedAtDoor.Toggle();
                }
            }
            
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
        
        private bool CheckPlayerCollision(Vector3 position)
        {
            // Create player bounding box at the new position
            BoundingBox playerBox = BoundingBox.FromCenterAndSize(position, _playerSize, Vector3.One);
            
            // Check against all world objects
            foreach (var obj in _worldObjects)
            {
                if (obj.CheckCollision(playerBox))
                {
                    return true; // Collision detected
                }
            }
            
            return false; // No collision
        }
        
        private Door? GetLookedAtDoor(float maxDistance)
        {
            // Get camera forward direction
            float yaw = MathHelper.DegreesToRadians(_camera.Yaw);
            float pitch = MathHelper.DegreesToRadians(_camera.Pitch);
    
            Vector3 forward = new Vector3(
                MathF.Cos(yaw) * MathF.Cos(pitch),
                MathF.Sin(pitch),
                MathF.Sin(yaw) * MathF.Cos(pitch)
            );
            forward = Vector3.Normalize(forward);
    
            // Check if looking at door and within range
            Vector3 toDoor = _houseDoor.Position - _camera.Position;
            float distance = toDoor.Length;
    
            if (distance > maxDistance)
                return null;
    
            // Check if we're roughly looking at the door
            Vector3 toDoorNormalized = Vector3.Normalize(toDoor);
            float dot = Vector3.Dot(forward, toDoorNormalized);
    
            // If dot > 0.8, we're looking roughly at the door (within ~36 degrees)
            if (dot > 0.2f)
                return _houseDoor;
    
            return null;
        }
    }
}