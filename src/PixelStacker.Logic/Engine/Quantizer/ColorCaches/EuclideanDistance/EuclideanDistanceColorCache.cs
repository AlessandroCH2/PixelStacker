﻿using System.Drawing;
using System.Collections.Generic;
using PixelStacker.Logic.Engine.Quantizer.Helpers;
using PixelStacker.Logic.Engine.Quantizer.ColorCaches.Common;

namespace PixelStacker.Logic.Engine.Quantizer.ColorCaches.EuclideanDistance
{
    public class EuclideanDistanceColorCache : BaseColorCache
    {
        #region | Fields |

        private IList<Color> palette;

        #endregion

        #region | Properties |

        /// <summary>
        /// Gets a value indicating whether this instance is color model supported.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is color model supported; otherwise, <c>false</c>.
        /// </value>
        public override bool IsColorModelSupported
        {
            get { return true; }
        }

        #endregion

        #region | Constructors |

        /// <summary>
        /// Initializes a new instance of the <see cref="EuclideanDistanceColorCache"/> class.
        /// </summary>
        public EuclideanDistanceColorCache()
        {
            ColorModel = ColorModel.RedGreenBlue;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EuclideanDistanceColorCache"/> class.
        /// </summary>
        /// <param name="colorModel">The color model.</param>
        public EuclideanDistanceColorCache(ColorModel colorModel)
        {
            ColorModel = colorModel;
        }

        #endregion

        #region << BaseColorCache >>

        /// <summary>
        /// See <see cref="BaseColorCache.OnCachePalette"/> for more details.
        /// </summary>
        protected override void OnCachePalette(IList<Color> palette)
        {
            this.palette = palette;
        }

        /// <summary>
        /// See <see cref="BaseColorCache.OnGetColorPaletteIndex"/> for more details.
        /// </summary>
        protected override void OnGetColorPaletteIndex(Color color, out int paletteIndex)
        {
            paletteIndex = ColorModelHelper.GetEuclideanDistance(color, ColorModel, palette);
        }

        #endregion
    }
}