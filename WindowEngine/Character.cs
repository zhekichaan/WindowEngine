using System;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace WindowEngine;
public class Character
{
    private enum State { Idle, Walk, Jump, Punch }
    private readonly int _shader;

    // Animation timing
    private float _animTimer = 0f;
    private int _animFrame = 0; // column index

    // Physics
    public Vector2 Position { get; private set; }
    private float _velY = 0f;
    private const float Gravity = -2000f;
    private const float JumpVelocity = 800f;
    private const float WalkSpeed = 160f;
    private const float GroundY = 230f;     // ground position

    // Sprite-sheet geometry
    private const float FrameW = 120f;
    private const float FrameH = 85f;
    private const float Gap = 5f;
    private const int Columns = 4;
    private const float TotalW = FrameW + Gap;
    private const float SheetW = Columns * TotalW - Gap;
    private const float SheetH = 340f;
    
    // Important: this must match how you draw the sprite (your vertices use w=120,h=85 as half-extents).
    public readonly Vector2 HalfSize = new Vector2(35f, 42.5f);

    // Expose AABB (centered on Position)
    public AABB GetAABB()
    {
        // Position is the sprite center (because vertex positions are -w..w and you translate by Position).
        return new AABB(Position, HalfSize);
    }

    // State machine
    private State _state = State.Idle;
    public Direction Facing { get; private set; } = Direction.Right;

    private readonly Dictionary<State, (int Row, float FrameTime, int Cols, bool Loop)> _anims;

    public Character(int shader)
    {
        _shader = shader;
        Position = new Vector2(400f, GroundY);

        _anims = new Dictionary<State, (int, float, int, bool)> {
            { State.Idle,  (3, 0.25f, Columns, true) }, // Idle row index 3
            { State.Walk,  (0, 0.12f, Columns, true) }, // Walk row index 0
            { State.Jump,  (1, 0.12f, Columns, false) }, // Jump row index 1, no loop
            { State.Punch, (2, 0.12f, Columns, false) } // Punch row index 2, no loop
        };

        SetState(State.Idle, true);
    }

    private bool _isOnGround => Math.Abs(Position.Y - GroundY) < 0.001f;

    public void HandleInput(float delta, bool leftHeld, bool rightHeld, bool upEdge, bool punchEdge)
    {
        State next = _state;

        // If currently in Jump: stay in Jump until landing
        if (_state == State.Jump)
        {
            next = State.Jump;
        }
        // If currently in Punch: play punch to completion (we don't interrupt)
        else if (_state == State.Punch)
        {
            next = State.Punch;
        }
        else
        {
            // Punch edge takes priority (one-shot)
            if (punchEdge)
            {
                next = State.Punch;
            }
            else if (upEdge && _isOnGround)
            {
                next = State.Jump;
            }
            else if (leftHeld || rightHeld)
            {
                next = State.Walk;
            }
            else
            {
                next = State.Idle;
            }
        }

        if (next != _state)
            SetState(next, true);

        UpdateStateBehavior(delta, leftHeld, rightHeld);
    }

    private void SetState(State s, bool resetAnim)
    {
        _state = s;
        if (resetAnim)
        {
            _animTimer = 0f;
            _animFrame = 0;
        }

        // If player just jumped, and on the ground, give vertical velocity
        if (s == State.Jump && _isOnGround)
        {
            _velY = JumpVelocity;
            // add some vertical position so onGround check doesn't refire
            Position = new Vector2(Position.X, Position.Y + 0.001f);
        }
    }

    private void UpdateStateBehavior(float delta, bool leftHeld, bool rightHeld)
    {
        if (_state == State.Punch)
        {
            // do nothing to Position.X
        }
        else if (_state != State.Jump)
        {
            float dir = 0f;
            if (leftHeld) dir -= 1f;
            if (rightHeld) dir += 1f;
            Position = new Vector2(Position.X + dir * WalkSpeed * delta, Position.Y);
            if (dir < 0) Facing = Direction.Left;
            else if (dir > 0) Facing = Direction.Right;
        }

        // Physics: gravity applies while Jumping
        if (_state == State.Jump || !_isOnGround)
        {
            _velY += Gravity * delta;
            float dir = 0f;
            if (leftHeld) dir -= 1f;
            if (rightHeld) dir += 1f;
            Position = new Vector2(Position.X + dir * WalkSpeed * delta, Position.Y + _velY * delta);

            // Landing detection
            if (Position.Y <= GroundY)
            {
                Position = new Vector2(Position.X, GroundY);
                _velY = 0f;
                // On landing, go to Idle
                SetState(State.Idle, true);
            }

            StepAnimation(delta); // play jump anim while airborne
            return;
        }

        // If on ground and not jumping:
        StepAnimation(delta);
    }

    private void StepAnimation(float delta)
    {
        var anim = _anims[_state];
        _animTimer += delta;
        if (_animTimer >= anim.FrameTime)
        {
            _animTimer -= anim.FrameTime;
            _animFrame++;
            if (anim.Loop)
                _animFrame %= anim.Cols;
            else
                _animFrame = Math.Min(_animFrame, anim.Cols - 1);
        }
        
        if (_state == State.Punch && _animFrame >= anim.Cols - 1) {
            if (_animTimer >= anim.FrameTime * 0.5f)
                SetState(State.Idle, true);
        }
        SetFrame(_animFrame, anim.Row);
    }

    public void Render()
    {
        SetFrame(_animFrame, _anims[_state].Row);
        
        GL.DrawArrays(PrimitiveType.TriangleFan, 0, 4);
    }

    private void SetFrame(int col, int row)
    {
        float x = (col * TotalW) / SheetW; // normalized start U
        float y = (row * FrameH) / SheetH; // normalized start V
        float w = FrameW / SheetW;         // normalized width
        float h = FrameH / SheetH;         // normalized height

        GL.UseProgram(_shader);
        int off = GL.GetUniformLocation(_shader, "uOffset");
        int sz = GL.GetUniformLocation(_shader, "uSize");
        GL.Uniform2(off, x, y);
        GL.Uniform2(sz, w, h);
    }
    
    public void ResolveCollision(Vector2 mtv)
    {
        // Only apply horizontal corrections (ignore Y)
        if (MathF.Abs(mtv.X) > MathF.Abs(mtv.Y))
        {
            Position = new Vector2(Position.X + mtv.X, Position.Y);

            if (_state == State.Walk)
                SetState(State.Idle, true);
        }
    }
}
