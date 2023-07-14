using System;
using System.Drawing;
using System.Linq;

namespace WinformsVisualization.Visualization
{
	internal class GradientCalculator
	{
		private Color[] _colors;

		public Color[] Colors
		{
			get
			{
				Color[] arg_19_0;
				if ((arg_19_0 = this._colors) == null)
				{
					arg_19_0 = (this._colors = new Color[0]);
				}
				return arg_19_0;
			}
			set
			{
				this._colors = value;
			}
		}

		public GradientCalculator()
		{
		}

		public GradientCalculator(params Color[] colors)
		{
			this._colors = colors;
		}

		public Color GetColor(float perc)
		{
			if (this._colors.Length > 1)
			{
				int num = Convert.ToInt32((float)(this._colors.Length - 1) * perc - 0.5f);
				float num2 = perc % (1f / (float)(this._colors.Length - 1)) * (float)(this._colors.Length - 1);
				if (num + 1 >= this.Colors.Length)
				{
					num = this.Colors.Length - 2;
				}
				return Color.FromArgb(255, (int)((byte)((float)this._colors[num + 1].R * num2 + (float)this._colors[num].R * (1f - num2))), (int)((byte)((float)this._colors[num + 1].G * num2 + (float)this._colors[num].G * (1f - num2))), (int)((byte)((float)this._colors[num + 1].B * num2 + (float)this._colors[num].B * (1f - num2))));
			}
			return this._colors.FirstOrDefault<Color>();
		}
	}
}
