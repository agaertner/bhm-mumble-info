using Blish_HUD.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Nekres.Mumble_Info.Core.UI.Controls {
    internal class DynamicLabel : Label {

        private Func<string> _target;
        public Func<string> Target {
            get => _target;
            set {
                if (SetProperty(ref _target, value)) {

                }
            }
        }

        public DynamicLabel(Func<string> target) {
            _target = target;
        }

        public override void Draw(SpriteBatch spriteBatch, Rectangle drawBounds, Rectangle scissor) {
            this.Text = _target?.Invoke() ?? this.Text;
            base.Draw(spriteBatch, drawBounds, scissor);
        }
    }
}
