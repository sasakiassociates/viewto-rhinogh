// using System;
// using System.Collections.Generic;
// using System.IO;
// using System.Linq;
// using Grasshopper.Kernel;
// using ViewTo;
// using ViewTo.StudyObject;
// using ViewToRhinoGh.Goo;
//
// namespace ViewToRhinoGh.Points
// {
//   public class SimpleResultReader : GH_Component
//   {
//     public SimpleResultReader() : base(
//       "Simple Result Reader",
//       "SRR",
//       "Test node for reading results from disk",
//       ConnectorInfo.CATEGORY,
//       ConnectorInfo.Nodes.RESULTS)
//     { }
//
//     public override Guid ComponentGuid => new Guid("f1ccf629-44b1-45db-bff0-60fa8f2ae5b4");
//
//     protected override void RegisterInputParams(GH_InputParamManager pManager)
//     {
//       pManager.AddTextParameter("path", "p", "path to file", GH_ParamAccess.item);
//     }
//
//     protected override void RegisterOutputParams(GH_OutputParamManager pManager)
//     {
//       pManager.AddParameter(new ViewObjParam("ResultObj", "RC", "View Obj as ViewObj Parameter Object", GH_ParamAccess.item));
//     }
//
//     private ResultCloud GetResults(string path)
//     {
//
//       if (!File.Exists(path))
//         return null;
//
//       var rc = new ResultCloud
//       {
//         data = new List<IResultData>()
//       };
//
//       var lines = File.ReadAllLines(path);
//
//       var header = lines[0].Split(',');
//
//       var ptns = new List<CloudPoint>();
//       for (var i = 1; i < lines.Length; i++)
//       {
//         var line = lines[i].Split(',');
//         ptns.Add(new CloudPoint(
//                    double.Parse(line[0]), double.Parse(line[1]), double.Parse(line[2]))
//         );
//       }
//
//       rc.points = ptns.ToArray();
//
//       for (var colIndex = 7; colIndex < header.Length; colIndex++)
//       {
//         var contentName = header[colIndex].Split('-').First();
//         var stageName = header[colIndex].Split('-').Last();
//
//         var values = new List<double>();
//         for (var i = 1; i < lines.Length; i++)
//           values.Add(double.Parse(lines[i].Split(',')[colIndex]));
//
//         var d = new ContentResultData(values, stageName, contentName, 0);
//         rc.data.Add(d);
//       }
//       return rc;
//     }
//
//     protected override void SolveInstance(IGH_DataAccess DA)
//     {
//
//       var path = string.Empty;
//       DA.GetData(0, ref path);
//
//       var rc = GetResults(path);
//
//       DA.SetData(0, rc);
//
//     }
//   }
// }
