using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Grasshopper.Kernel;
using Rhino.Geometry;
using ViewObjects.Cloud;
using ViewTo.RhinoGh.Goo;
using ViewTo.RhinoGh.Properties;

namespace ViewTo.RhinoGh.Results
{
	public class ExploreResults : GH_Component
	{
		public ExploreResults() : base(
			"Explore Results",
			"ExR",
			"Result explorer",
			ConnectorInfo.CATEGORY,
			ConnectorInfo.Nodes.RESULTS)
		{ }

		public override Guid ComponentGuid => new Guid("01ffe845-0a7b-4bf8-9d35-48f234fc8cfc");

		protected override Bitmap Icon => new Bitmap(Resources.ExploreResults);

		private int _iObj, _iTarget, _iStageInput, _iScoreRange, _iColors, _iSize;
		protected override void RegisterInputParams(GH_InputParamManager pManager)
		{
			var index = 0;
			pManager.AddGenericParameter("ResultCloud", "R", "ResultCloud for Study", GH_ParamAccess.item);
			_iObj = index++;
			pManager.AddTextParameter("View Target", "T", "Target content to use", GH_ParamAccess.item);
			_iTarget = index++;
			pManager.AddTextParameter("Stage", "S", "Stage for pixel data", GH_ParamAccess.item);
			_iStageInput = index++;
			pManager.AddColourParameter("Colors", "C", "Ordered list of colors for use with display.", GH_ParamAccess.list);
			_iColors = index++;
			pManager.AddIntegerParameter("Point Size", "P", "Size of Point in Cloud", GH_ParamAccess.item, 3);
			_iSize = index;

			pManager[_iColors].Optional = true;
			pManager[_iSize].Optional = true;
		}

		protected override void RegisterOutputParams(GH_OutputParamManager pManager)
		{
			pManager.AddPointParameter("Points", "P", "Points from filter", GH_ParamAccess.list);
			pManager.AddColourParameter("Color", "C", "Color from filter", GH_ParamAccess.list);
			pManager.AddNumberParameter("Values", "V", "Values from filter", GH_ParamAccess.list);
		}

		public override void DrawViewportWires(IGH_PreviewArgs args)
		{
			if (_pc != null)
				args.Display.DrawPointCloud(_pc, _pointSize);
		}

		private List<Color> _storedColors;
		private PointCloud _pc;
		private int _pointSize;

		protected override void SolveInstance(IGH_DataAccess DA)
		{
			GH_ViewObj wrapper = null;
			DA.GetData(_iObj, ref wrapper);

			if (wrapper?.Value is ResultCloud resultCloud)
			{
				var contentName = string.Empty;
				DA.GetData(_iTarget, ref contentName);
				var baseStageName = string.Empty;
				DA.GetData(_iStageInput, ref baseStageName);

				var explorer = new ResultExplorer();
				explorer.Load(resultCloud);

				_storedColors = new List<Color>();

				if (!DA.GetDataList(_iColors, _storedColors))
					_storedColors = new List<Color>
					{
						Color.FromArgb(255, 54, 2, 89),
						Color.FromArgb(255, 92, 0, 91),
						Color.FromArgb(255, 127, 0, 86),
						Color.FromArgb(255, 158, 0, 75),
						Color.FromArgb(255, 183, 0, 59),
						Color.FromArgb(255, 202, 0, 38),
						Color.FromArgb(255, 213, 0, 0)
					};

				var values = explorer.Fetch(ResultType.Potential).ToList();
				var points = new List<Point3d>();
				var colors = new List<Color>();

				for (var i = 0; i < values.Count; i++)
				{
					var value = values[i];
					var ptn = resultCloud.points[i];
					points.Add(new Point3d(ptn.x, ptn.y, ptn.z));

					Color color = default;
					if (value < 0)
					{
						color = Color.Gray;
					}
					else if (value == 0)
					{
						color = Color.Black;
					}
					else
					{
						var num = (int)Math.Round((double)((_storedColors.Count - 1) * value), 0);
						color = _storedColors[num];
					}

					colors.Add(color);
				}

				if (!DA.GetData(_iSize, ref _pointSize))
					_pointSize = 3;

				_pc = new PointCloud();
				_pc.ClearColors();

				for (var i = 0; i < points.Count; i++)
					_pc.Add(points[i], colors[i]);

				DA.SetDataList(0, points);
				DA.SetDataList(1, colors);
				DA.SetDataList(2, values);
			}
		}

	}
}