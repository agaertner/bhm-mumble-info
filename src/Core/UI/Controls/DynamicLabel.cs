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

        private Color _textDataColor;
        public Color TextDataColor {
            get => _textDataColor;
            set {
                if (SetProperty(ref _textDataColor, value)) {

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

        private bool _strokeTextData;
        public bool StrokeTextData {
            get => _strokeTextData;
            set {
                if (SetProperty(ref _strokeTextData, value)) {

                }
            }
        }

        public DynamicLabel(Func<string> textData) {
            _textDataColor = _textColor;
            _textData      = textData;
        }

        protected override void Paint(SpriteBatch spriteBatch, Rectangle bounds) {
            var size = _font.MeasureString(string.IsNullOrEmpty(_text) ? "." : _text);
            var width = string.IsNullOrEmpty(_text) ? 0 : (int)Math.Round(size.Width);
            var iconSize = (int)Math.Round(size.Height); // Set icon size to fit height of text.

            if (_icon is { HasSwapped: true, HasTexture: true }) {
                spriteBatch.DrawOnCtrl(this, _icon, new Rectangle(0, (bounds.Height - iconSize) / 2, iconSize, iconSize));
                bounds = new Rectangle(bounds.X + iconSize, bounds.Y, bounds.Width - iconSize, bounds.Height); // Pad text to the right of the icon.
            }

            base.Paint(spriteBatch, bounds); // Draw normal static text first as a prefix.

            var textData = _textData?.Invoke(); // Fetch dynamic text data.

            if (string.IsNullOrEmpty(textData)) {
                LoadingSpinnerUtil.DrawLoadingSpinner(this, spriteBatch, new Rectangle(width + iconSize, (bounds.Height - iconSize) / 2, iconSize, iconSize));
                return;
            }

            // Draw text data to the right of the normal static text.
            bounds = new Rectangle(bounds.X + width + 1, bounds.Y, bounds.Width - width, bounds.Height);
            spriteBatch.DrawStringOnCtrl(this, textData, _font, bounds, _textDataColor, _wrapText, _strokeTextData, 1, _horizontalAlignment, _verticalAlignment);
        }
    }
}
