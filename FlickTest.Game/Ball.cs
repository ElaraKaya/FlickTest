using System;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Effects;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;
using osuTK;
using osuTK.Graphics;
using osuTK.Input;

namespace FlickTest.Game
{
    public partial class Ball : Circle
    {
        private Vector2 velocity;
        private bool dragging;
        private Vector2 dragOffset;
        private Vector2 smoothVelocity;
        private double lastDragTime;

        public Ball()
        {
            Size = new Vector2(100);
            Colour = Color4.White;
            Anchor = Anchor.Centre;
            Origin = Anchor.Centre;
            EdgeEffect = new EdgeEffectParameters
            {
                Type = EdgeEffectType.Glow,
                Colour = new Color4(0.5f, 0.3f, 1f, 0.6f),
                Radius = 30,
            };
        }

        protected override bool OnMouseDown(MouseDownEvent e)
        {
            // 中键/右键删除这个球
            if (e.Button is MouseButton.Middle or MouseButton.Right)
                Expire();

            // 左键也返回 true，使本球进入 ButtonDownInputQueue，拖拽才能触发
            return true;
        }

        protected override bool OnDragStart(DragStartEvent e)
        {
            dragging = true;
            dragOffset = Position - e.MousePosition;
            velocity = Vector2.Zero;
            smoothVelocity = Vector2.Zero;
            lastDragTime = Time.Current;
            this.ScaleTo(1.25f, 200, Easing.OutQuint);
            return true;
        }

        protected override void OnDrag(DragEvent e)
        {
            Position = e.MousePosition + dragOffset;

            double currentTime = Time.Current;
            float dt = (float)Math.Max((currentTime - lastDragTime) / 1000.0, 0.001);
            lastDragTime = currentTime;

            Vector2 instantVelocity = e.Delta / dt;
            smoothVelocity = Vector2.Lerp(smoothVelocity, instantVelocity, 1 - MathF.Pow(0.01f, dt));
        }

        protected override void OnDragEnd(DragEndEvent e)
        {
            dragging = false;
            velocity = smoothVelocity;
            this.ScaleTo(1f, 300, Easing.OutQuint);
        }

        protected override void Update()
        {
            base.Update();

            if (dragging || Parent == null) return;

            float dt = (float)(Clock.ElapsedFrameTime / 1000.0);
            if (dt <= 0) return;

            Position += velocity * dt;

            // 帧率无关的时间衰减摩擦力 (每秒大约衰减 60%)
            velocity *= MathF.Pow(0.4f, dt);

            if (velocity.Length < 10f)
                velocity = Vector2.Zero;

            // Anchor.Centre 下的活动范围
            float boundX = Math.Max(0, (Parent.DrawWidth - DrawWidth) / 2f);
            float boundY = Math.Max(0, (Parent.DrawHeight - DrawHeight) / 2f);

            if (X < -boundX)
            {
                X = -boundX;
                velocity.X = MathF.Abs(velocity.X) * 0.75f;
            }
            else if (X > boundX)
            {
                X = boundX;
                velocity.X = -MathF.Abs(velocity.X) * 0.75f;
            }

            if (Y < -boundY)
            {
                Y = -boundY;
                velocity.Y = MathF.Abs(velocity.Y) * 0.75f;
            }
            else if (Y > boundY)
            {
                Y = boundY;
                velocity.Y = -MathF.Abs(velocity.Y) * 0.75f;
            }
        }
    }
}
