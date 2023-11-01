using mathgem;
using System.Xml.Linq;

namespace drawpolys
{
	class drawer
	{
		public static Image<Rgba32> drawImg(poly3d poly) { return drawImg(new shape3d(poly)); }
		public static Image<Rgba32> drawImg(List<poly3d> polys) { return drawImg(new shape3d(polys)); }
		public static Image<Rgba32> drawImg(shape3d shape)
		{
			if (shape.isNull) return new Image<Rgba32>(1, 1);
			if (shape.polys().Count == 0) return new Image<Rgba32>(1, 1);
			Rectangle rect = shape.rect();
			if (rect.IsEmpty) return new Image<Rgba32>(1, 1);
			var img = new Image<Rgba32>(rect.Width + 2, rect.Height + 2);
			List<poly3d> polys = shape.polys();
			Console.WriteLine($"\nTotal polygons: {shape.polys().Count}");
			if (calculationSettings.multiTask)
			{
				int areasumm = 0;
				List<int> areas = new List<int>(polys.Count);
				for (int i = 0; i < polys.Count; i++)
				{
					int a = Math.Min(polys[i].rect.Width, polys[i].rect.Height);
					areas.Add(a);
					areasumm += a;
				}
				Console.WriteLine($"Total area: {areasumm}");
				int streams = Math.Min(4, polys.Count / 2); int targetarea = areasumm / streams;
				int[] divided = new int[streams];
				List<List<int>> indexes = new List<List<int>>(streams);
				for (int i = 0; i < streams; i++)
				{ divided[i] = 0; indexes.Add(new List<int>()); }
				for (int i = 0; i < areas.Count; i++)
				{
					List<int> diffs = new List<int>(streams);
					for (int j = 0; j < streams; j++)
					{
						diffs.Add(targetarea - divided[j] - areas[i]);
					}
					int index = diffs.IndexOf(diffs.Max());
					divided[index] += areas[i];
					indexes[index].Add(i);
				}

				Console.WriteLine($"divided areas into {streams} stream(s):");
				Console.WriteLine($"desired: {targetarea}");
				for (int i = 0; i < streams; i++)
				{
					string show = "";
					for (int j = 0; j < indexes[i].Count; j++)
						show += $"{indexes[i][j]}, ";
					show += $" : area = {divided[i]}";
					Console.WriteLine(show);
				}

				Task[] tasks = new Task[streams];
				for (int i = 0; i < streams; i++)
				{
					int _ = i;
					tasks[_] = Task.Run(() =>
					{
						List<List<Point>> allpoints = new List<List<Point>>(indexes.Count);
						foreach (int j in indexes[_])
						{
							allpoints.Add(polys[j].allPoints());
						}
						List<Color> colors = new List<Color>();
						for (int u = 0; u < allpoints.Count; u++)
							colors.Add(new Color(new Rgba32((byte)math.rnd.Next(256), (byte)math.rnd.Next(256), (byte)math.rnd.Next(256))));
						int offX = rect.Left; int offY = rect.Top;
						for (int u = 0; u < allpoints.Count; u++)
						{
							for (int v = 0; v < allpoints[u].Count; v++)
							{
								int x = allpoints[u][v].X - offX;
								int y = allpoints[u][v].Y - offY;
								img[x, y] = colors[u];
							}
						}
					});
				}
				Task.WaitAll(tasks);
			}
			else
			{
				List<List<Point>> allpoints = polys.Select(zxc => new List<Point>(zxc.allPoints())).ToList();//new List<List<Point>>(polys.Count);
																											 //for (int i = 0; i < polys.Count; i++)
																											 //	allpoints.Add(polys[i].allPoints());
				List<Color> colors = new List<Color>();
				for (int i = 0; i < allpoints.Count; i++)
					colors.Add(new Color(new Rgba32((byte)math.rnd.Next(256), (byte)math.rnd.Next(256), (byte)math.rnd.Next(256))));
				int offX = rect.Left; int offY = rect.Top;
				for (int i = 0; i < allpoints.Count; i++)
				{
					for (int j = 0; j < allpoints[i].Count; j++)
					{
						int x = allpoints[i][j].X - offX;
						int y = allpoints[i][j].Y - offY;
						img[x, y] = colors[i];
					}
				}
			}
			return img;
		}
	}
}