using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LyricPlayer.Model.Elements.Enums
{
	[Flags]
	public enum SpectrumVisualizationType
	{
		Bottom = 1,
		Top = 2,
		Left = 4,
		Right = 8,
	}
}
