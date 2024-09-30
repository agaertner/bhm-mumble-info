using Blish_HUD;
using Blish_HUD.Content;
using Blish_HUD.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Nekres.Mumble_Info.Core.UI.Controls {
    internal class DynamicLabel : Label {

        private AsyncTexture2D _icon;
        public AsyncTexture2D Icon {
            get => _icon;
            set {
                if (SetProperty(ref _icon, value)) {

                }
            }
        }

        private string _prefix;
        public string Prefix {
            get => _prefix;
            set {
                if (SetProperty(ref _prefix, value)) {

                }
            }
        }

        private Func<string> _textData;
        public Func<string> TextData {
            get => _textData;
            set {
                if (SetProperty(ref _textData, value)) {

                }
            }
        }

        public DynamicLabel(Func<string> textData) {
            _textData = textData;
        }

        public override void Draw(SpriteBatch spriteBatch, Rectangle drawBounds, Rectangle scissor) {
            spriteBatch.Begin();

            var size   = _font.MeasureString(".");
            var height = (int)Math.Round(size.Height);
            var width  = (int)Math.Round(size.Width);

            var prefix = _prefix;
            if (_icon is {HasSwapped: true, HasTexture: true}) {
                prefix = "    " + prefix;
                spriteBatch.DrawOnCtrl(this, _icon, new Rectangle(0,0,height,height));
            }

            //TODO: Bugfix wrong pos for icon and possibly loading spinner
            
            var textData = _textData?.Invoke();
            if (string.IsNullOrEmpty(textData)) {
                LoadingSpinnerUtil.DrawLoadingSpinner(this, spriteBatch, new Rectangle(width + 2, 0, height, height));
            }
            this.Text = prefix + textData;
            spriteBatch.End();
            base.Draw(spriteBatch, drawBounds, scissor);
        }
    }
}
