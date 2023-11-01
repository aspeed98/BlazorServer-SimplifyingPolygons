namespace mathgem
{
	public static class math
	{
		public static Random rnd = new Random();
		public static bool isNaN(double x)
		{
			return Double.IsNaN(x) || Double.IsInfinity(x);
		}
		public static bool same(double x, double y)
		{
			if (x == y) return true;
			double diff = Math.Abs(x - y);
			//double tolerance = 0.001;
			double tolerance = 0.000001;
			double mindiff = 0.001;
			//double mindiff = 0.000001;
			return diff <= mindiff || (Math.Abs(diff / x) <= tolerance && Math.Abs(diff / y) <= tolerance);
		}
		public static bool same(double x, double y, double mindiff)
		{
			if (x == y) return true;
			double diff = Math.Abs(x - y);
			return diff <= mindiff;
		}
		public static double deter(double[] m)
		{
			if (m.Length == 4)
				return m[0] * m[3] - (m[1] * m[2]);
			if (m.Length == 9)
				return m[0] * m[4] * m[8] + m[1] * m[6] * m[5] + m[2] * m[3] * m[7] - (m[2] * m[4] * m[6] + m[1] * m[3] * m[8] + m[0] * m[5] * m[7]);
			if (m.Length == 16)
			{
				double d0 = deter(new double[] { m[5], m[6], m[7], m[9], m[10], m[11], m[13], m[14], m[15] });
				double d1 = deter(new double[] { m[4], m[6], m[7], m[8], m[10], m[11], m[12], m[14], m[15] });
				double d2 = deter(new double[] { m[4], m[5], m[7], m[8], m[9], m[11], m[12], m[13], m[15] });
				double d3 = deter(new double[] { m[4], m[5], m[6], m[8], m[9], m[10], m[12], m[13], m[14] });
				return m[0] * d0 - m[1] * d1 + m[2] * d2 - m[3] * d3;
			}
			return 0;
		}
		public static double angle(point3d A, point3d B, point3d C, bool? right)
		{
			return angle(A, B, C, right, new poly3d(A, B, C).norm);
		}
		public static double angle(point3d A, point3d B, point3d C, bool? right, point3d norm)
		{
			if (norm.isNull || math.same(norm.z, 0) || right == null)
			{
				return 360.0;
			}
			point3d v1 = B - A;
			point3d v2 = C - B;
			double scaleX = Math.Sqrt(1 - norm.x * norm.x);
			double scaleY = Math.Sqrt(1 - norm.y * norm.y);
			double v1x = v1.x / scaleX;
			double v1y = v1.y / scaleY;
			double v2x = v2.x / scaleX;
			double v2y = v2.y / scaleY;
			double angle1 = calcangleDeg(v1x, v1y);
			double angle2 = calcangleDeg(v2x, v2y);
			double angle = minAngleDiff(angle1, angle2);
			if (right == true)
			{
				if (insideSector(angle2, angle1 - 180.0, angle1))
					return 180.0 - angle;
				else
					return 180.0 + angle;
			}
			else
			{
				if (insideSector(angle2, angle1, angle1 + 180.0))
					return 180.0 - angle;
				else
					return 180.0 + angle;
			}
			double calcangleDeg(double dx, double dy)
			{
				double angle = Math.Atan2(dy, dx) * 180.0 / Math.PI;
				while (angle < 0.0) angle += 360.0;
				return angle;
			}
			bool insideSector(double angle, double start, double end)
			{
				while (start < 0.0)
				{
					start += 360.0;
					end += 360.0;
				}
				while (end < start)
					end += 360.0;
				while (angle < start)
					angle += 360.0;
				return angle >= start && angle <= end;
			}
			double minAngleDiff(double angle1, double angle2)
			{
				return Math.Min(Math.Abs(angle1 - angle2), Math.Min(Math.Abs(angle1 - angle2 - 360), Math.Abs(angle1 - angle2 + 360)));
			}
		}
		public static bool? clockwiseDirection(List<point3d> points)
		{
			if (points.Count < 3)
				return null;
			return clockwiseDirection(points, new poly3d(points[0], points[1], points[2]).norm);
		}
		public static bool? clockwiseDirection(List<point3d> points, point3d norm)
		{
			if (norm.isNull || math.same(norm.z, 0) || points.Count < 3)
			{
				return null;
			}
			double shouldSumm = (points.Count - 2) * 180;
			double angleSummR = 0;
			double angleSummL = 0;
			for (int i = 0; i < points.Count; i++)
			{
				int l = i == 0 ? points.Count - 1 : i - 1;
				int r = i == points.Count - 1 ? 0 : i + 1;
				angleSummR += angle(points[l], points[i], points[r], true, norm);
			}
			for (int i = 0; i < points.Count; i++)
			{
				int l = i == 0 ? points.Count - 1 : i - 1;
				int r = i == points.Count - 1 ? 0 : i + 1;
				angleSummL += angle(points[l], points[i], points[r], false, norm);
			}
			double diffR = Math.Abs(angleSummR - shouldSumm);
			double diffL = Math.Abs(angleSummL - shouldSumm);
			if (diffR < diffL)
				return true;
			else
				return false;
		}
	}
	public static class calculationSettings
	{
		public static int simplificationMethod = 2;
		public static bool autoCalculate = true;
		public static bool multiTask = true;
	}
	public struct rect2d
	{
		public double left { get; set; }
		public double right { get; set; }
		public double width { get; set; }
		public double top { get; set; }
		public double bottom { get; set; }
		public double height { get; set; }
		public double area { get; set; }
		public bool isNull { get; set; }
		public rect2d(double left, double top, double right, double bottom)
		{
			this.left = left; this.top = top; this.right = right; this.bottom = bottom;
			if (this.left > this.right) { this.left = right; this.right = left; }
			if (this.top > this.bottom) { this.top = bottom; this.bottom = top; }
			this.width = this.right - this.left;
			this.height = this.bottom - this.top;
			this.area = Math.Max(1.0, this.width) * Math.Max(1.0, this.height);
			this.isNull = false;
		}
		public rect2d()
		{
			this.left = new double(); this.top = new double(); this.right = new double(); this.bottom = new double();
			this.width = new double(); this.height = new double();
			this.area = new double();
			this.isNull = true;
		}
		public bool intersect(rect2d rect)
		{
			if (this.isNull || rect.isNull)
				return false;
			return !(rect.left > this.right || this.left > rect.right || rect.top > this.bottom || this.top > rect.bottom);
		}
		public bool pointInbound(point3d p)
		{
			if (this.isNull || p.isNull)
				return false;
			return p.x <= right && p.x >= left && p.y <= bottom && p.y >= top;
		}
		public bool lineInboud(line3d line)
		{
			if (this.isNull || line.isNull)
				return false;
			return pointInbound(line.A) && pointInbound(line.B);
		}
		public bool polyInboud(poly3d poly)
		{
			if (this.isNull || poly.isNull)
				return false;
			return pointInbound(poly.A) && pointInbound(poly.B) && pointInbound(poly.C);
		}
		public bool rectInbound(rect2d rect)
		{
			if (this.isNull || rect.isNull)
				return false;
			point3d p1 = new point3d(rect.left, rect.top);
			point3d p2 = new point3d(rect.right, rect.bottom);
			return pointInbound(p1) && pointInbound(p2);
		}
	}
	public struct rect3d
	{
		public double left { get; set; }
		public double right { get; set; }
		public double width { get; set; }
		public double top { get; set; }
		public double bottom { get; set; }
		public double height { get; set; }
		public double front { get; set; }
		public double back { get; set; }
		public double depth { get; set; }
		public double area { get; set; }
		public double volume { get; set; }
		public bool isNull { get; set; }
		public rect3d(double left, double top, double front, double right, double bottom, double back)
		{
			this.left = left; this.top = top; this.front = front; this.right = right; this.bottom = bottom; this.back = back;
			if (this.left > this.right) { this.left = right; this.right = left; }
			if (this.top > this.bottom) { this.top = bottom; this.bottom = top; }
			if (this.front > this.back) { this.front = back; this.back = front; }
			this.width = this.right - this.left;
			this.height = this.bottom - this.top;
			this.depth = this.back - this.front;
			this.area = Math.Max(1.0, this.width) * Math.Max(1.0, this.height);
			this.volume = Math.Max(1.0, this.width) * Math.Max(1.0, this.height) * Math.Max(1.0, this.depth);
			this.isNull = false;
		}
		public rect3d()
		{
			this.left = new double(); this.top = new double(); this.front = new double(); this.right = new double(); this.bottom = new double(); this.back = new double();
			this.width = new double(); this.height = new double(); this.depth = new double();
			this.area = new double(); this.volume = new double();
			this.isNull = true;
		}
		public bool intersect(rect3d rect)
		{
			if (this.isNull || rect.isNull)
				return false;
			return !(rect.left > this.right || this.left > rect.right || rect.top > this.bottom || this.top > rect.bottom || rect.front > this.back || this.front > rect.back);
		}
		public bool pointInbound(point3d p)
		{
			if (this.isNull || p.isNull)
				return false;
			return (p.x <= right || math.same(p.x, right, 0.001)) &&
				(p.x >= left || math.same(p.x, left, 0.001)) &&
				(p.y <= bottom || math.same(p.y, bottom, 0.001)) &&
				(p.y >= top || math.same(p.y, top, 0.001)) &&
				(p.z <= back || math.same(p.z, back, 0.001)) &&
				(p.z >= front || math.same(p.z, front, 0.001));
		}
		public bool lineInboud(line3d line)
		{
			if (this.isNull || line.isNull)
				return false;
			return pointInbound(line.A) && pointInbound(line.B);
		}
		public bool polyInboud(poly3d poly)
		{
			if (this.isNull || poly.isNull)
				return false;
			return pointInbound(poly.A) && pointInbound(poly.B) && pointInbound(poly.C);
		}
		public bool rectInbound(rect3d rect)
		{
			if (this.isNull || rect.isNull)
				return false;
			point3d p1 = new point3d(rect.left, rect.top, rect.front);
			point3d p2 = new point3d(rect.right, rect.bottom, rect.back);
			return pointInbound(p1) && pointInbound(p2);
		}
	}
	public struct point3d // either point or vector
	{
		public double x { get; set; }
		public double y { get; set; }
		public double z { get; set; }
		public double length { get; set; }
		public bool isNull { get; set; }
		public point3d() { toNull(); }
		public point3d(double x, double y)
		{
			this.isNull = false;
			if (math.isNaN(x) || math.isNaN(y))
			{
				toNull();
				return;
			}
			this.x = x;
			this.y = y;
			this.z = 0;
			this.length = Math.Sqrt(this.x * this.x + this.y * this.y);
		}
		public point3d(double x, double y, double z)
		{
			this.isNull = false;
			if (math.isNaN(x) || math.isNaN(y) || math.isNaN(z))
			{
				toNull();
				return;
			}
			this.x = x;
			this.y = y;
			this.z = z;
			this.length = Math.Sqrt(this.x * this.x + this.y * this.y + this.z * this.z);
		}
		public Point toPoint()
		{
			return new Point((int)Math.Round(this.x), (int)Math.Round(this.y));
		}
		public point3d vectorMult(point3d point)
		{
			if (this.isNull || point.isNull)
				return new point3d();
			return new point3d(this.y * point.z - this.z * point.y, this.z * point.x - this.x * point.z, this.x * point.y - this.y * point.x);
		}
		public double scalarMult(point3d point)
		{
			if (this.isNull || point.isNull)
				return 0;
			return this.x * point.x + this.y * point.y + this.z * point.z;
		}
		public bool inlist(List<point3d> points)
		{
			for (int i = 0; i < points.Count; i++)
				if (this == points[i])
					return true;
			return false;
		}
		public bool inbound(line3d line) { return inbound(line, true); }
		public bool inbound(line3d line, bool z)
		{
			return line.pointInbound(this, z);
		}
		public bool inbound(List<line3d> lines) { return inbound(lines, true); }
		public bool inbound(List<line3d> lines, bool z)
		{
			for (int i = 0; i < lines.Count; i++)
				if (lines[i].pointInbound(this, z))
					return true;
			return false;
		}
		public bool inbound(poly3d poly) { return inbound(poly, true); }
		public bool inbound(poly3d poly, bool z)
		{
			return poly.pointInbound(this, z);
		}
		public bool inbound(List<poly3d> polys) { return inbound(polys, true); }
		public bool inbound(List<poly3d> polys, bool z)
		{
			for (int i = 0; i < polys.Count; i++)
				if (polys[i].pointInbound(this, z))
					return true;
			return false;
		}
		public bool inside(line3d line) { return inside(line, true); }
		public bool inside(line3d line, bool z)
		{
			return line.pointInside(this, z);
		}
		public bool inside(List<line3d> lines) { return inside(lines, true); }
		public bool inside(List<line3d> lines, bool z)
		{
			for (int i = 0; i < lines.Count; i++)
				if (inside(lines[i], z))
					return true;
			return false;
		}
		public bool inside(poly3d poly) { return inside(poly, true); }
		public bool inside(poly3d poly, bool z)
		{
			return poly.pointInside(this, z);
		}
		public bool inside(List<poly3d> polys) { return inside(polys, true); }
		public bool inside(List<poly3d> polys, bool z)
		{
			for (int i = 0; i < polys.Count; i++)
				if (inside(polys[i], z))
					return true;
			return false;
		}
		public static point3d operator +(point3d A, point3d B)
		{
			if (A.isNull && B.isNull)
				return new point3d();
			if (A.isNull)
				return B;
			if (B.isNull)
				return A;
			return new point3d(A.x + B.x, A.y + B.y, A.z + B.z);
		}
		public static point3d operator -(point3d A, point3d B)
		{
			if (A.isNull && B.isNull)
				return new point3d();
			if (A.isNull)
				return B;
			if (B.isNull)
				return A;
			return new point3d(A.x - B.x, A.y - B.y, A.z - B.z);
		}
		public static point3d operator *(point3d A, double multiply)
		{
			if (math.same(multiply, 0) || math.isNaN(multiply) || A.isNull)
				return new point3d();
			return new point3d(A.x * multiply, A.y * multiply, A.z * multiply);
		}
		public static point3d operator /(point3d A, double divide)
		{
			if (math.same(divide, 0) || math.isNaN(divide) || A.isNull)
				return new point3d();
			return new point3d(A.x / divide, A.y / divide, A.z / divide);
		}
		public static bool operator ==(point3d A, point3d B)
		{
			if (A.isNull && B.isNull)
				return true;
			if (!A.isNull && !B.isNull)
				//if (math.same(A.x, B.x) && math.same(A.y, B.y) && math.same(A.z, B.z))
				if (math.same(A.x, B.x, 0.001) && math.same(A.y, B.y, 0.001) && math.same(A.z, B.z, 0.001))
					return true;
			return false;
		}
		public static bool operator !=(point3d A, point3d B)
		{
			return !(A == B);
		}
		public override bool Equals(object? obj)
		{
			if (obj == null)
				return false;
			if (!(obj is point3d))
				return false;
			return this == (point3d)obj;
		}
		public override int GetHashCode()
		{
			return Tuple.Create(x, y, z, isNull).GetHashCode();
		}
		public point3d cloneNoZ()
		{
			if (this.isNull)
				return new point3d();
			return new point3d(x, y, 0);
		}
		public point3d clone()
		{
			if (this.isNull)
				return new point3d();
			return new point3d(x, y, z);
		}
		public string str()
		{
			if (this.isNull)
				return "null";
			return $"X: {this.x.ToString("F2"),7} Y: {this.y.ToString("F2"),7} Z: {this.z.ToString("F2"),7}";
		}
		private void toNull()
		{
			this.x = new double();
			this.y = new double();
			this.z = new double();
			this.length = new double();
			this.isNull = true;
		}
	}
	public struct line3d
	{
		public point3d A { get; set; }
		public point3d B { get; set; }
		public point3d e { get; set; }
		private point3d centerpoint { get; set; }
		public rect3d rect3d { get; set; }
		public bool isNull { get; set; }
		public line3d() { toNull(); }
		public line3d(poly3d A, poly3d B)
		{
			if (A.isNull || B.isNull)
			{
				toNull();
				return;
			}
			if (!A.rect3d.intersect(B.rect3d))
			{
				toNull();
				return;
			}
			if (A.sameSurface(B))
			{
				toNull();
				return;
			}
			List<line3d> lines = A.lines();
			List<point3d> points = new List<point3d>();
			for (int i = 0; i < lines.Count; i++)
			{
				point3d toadd = lines[i].crossingPoint(B);
				if (!toadd.isNull && !toadd.inlist(points))
					points.Add(toadd);
				if (points.Count >= 2)
				{
					this = new line3d(points[0], points[1]);
					return;
				}
			}
			lines = B.lines();
			for (int i = 0; i < lines.Count; i++)
			{
				point3d toadd = lines[i].crossingPoint(A);
				if (!toadd.isNull && !toadd.inlist(points))
					points.Add(toadd);
				if (points.Count >= 2)
				{
					this = new line3d(points[0], points[1]);
					return;
				}
			}
			toNull();
		}
		public line3d(point3d A, point3d B)
		{
			if (A.isNull || B.isNull || A == B)
			{
				toNull();
				return;
			}
			this.A = A; this.B = B;
			point3d E = new point3d(B.x - A.x, B.y - A.y, B.z - A.z);
			double lenE = Math.Sqrt(E.x * E.x + E.y * E.y + E.z * E.z);
			if (math.same(lenE, 0))
			{
				toNull();
				return;
			}
			this.e = E / lenE;
			setCenter();
			setBounds();
			this.isNull = false;
		}
		private void setCenter()
		{
			this.centerpoint = (A + B) / 2.0;
		}
		private void setBounds()
		{
			double maxX = Math.Max(A.x, B.x);
			double minX = Math.Min(A.x, B.x);
			double maxY = Math.Max(A.y, B.y);
			double minY = Math.Min(A.y, B.y);
			double maxZ = Math.Max(A.z, B.z);
			double minZ = Math.Min(A.z, B.z);
			this.rect3d = new rect3d(minX, minY, minZ, maxX, maxY, maxZ);
		}
		public point3d t(double t) { return this.t(t, true); }
		public point3d t(double t, bool z)
		{
			return new point3d(this.e.x * t + A.x, this.e.y * t + A.y, z ? this.e.z * t + A.z : 0);
		}
		public point3d randPoint()
		{
			if (this.isNull)
				return new point3d();
			if (math.same((A - B).length, 0))
				return new point3d();
			double maxt = Math.Abs((A - B).length / this.e.length);
			int tries = 0;
			while (tries < 5)
			{
				point3d ret = this.t(math.rnd.NextDouble() * maxt, true);
				if (ret != A && ret != B)
					return ret;
				tries++;
			}
			return center();
		}
		public bool pointInbound(point3d p) { return pointInbound(p, true); }
		public bool pointInbound(point3d p, bool z)
		{
			if (this.isNull || p.isNull)
				return false;
			if (!z)
				return this.cloneNoZ().pointInbound(p.cloneNoZ(), true);
			if (p == A || p == B)
				return true;
			return this.rect3d.pointInbound(p);
		}
		public bool pointInside(point3d p) { return pointInside(p, true); }
		public bool pointInside(point3d p, bool z)
		{
			if (this.isNull || p.isNull)
				return false;
			if (!z)
				return this.cloneNoZ().pointInside(p.cloneNoZ(), true);
			if (p == A || p == B)
				return true;
			if (this.rect3d.pointInbound(p))
			{
				double s1 = (p.x - A.x) / e.x;
				double s2 = (p.y - A.y) / e.y;
				double s3 = (p.z - A.z) / e.z;
				bool b1 = math.same(e.x, 0);
				bool b2 = math.same(e.y, 0);
				bool b3 = math.same(e.z, 0);
				if (!b1 && !b2 && !b3)
					if (math.same(s1, s2) && math.same(s2, s3))
						return true;
					else return false;
				if (b1 && !b2 && !b3)
					if (math.same(s2, s3) && math.same(p.x, A.x))
						return true;
					else return false;
				if (!b1 && b2 && !b3)
					if (math.same(s1, s3) && math.same(p.y, A.y))
						return true;
					else return false;
				if (!b1 && !b2 && b3)
					if (math.same(s1, s2) && math.same(p.z, A.z))
						return true;
					else return false;
				if (b1 && b2 && !b3)
					if (math.same(p.x, A.x) && math.same(p.y, A.y))
						return true;
					else return false;
				if (b1 && !b2 && b3)
					if (math.same(p.x, A.x) && math.same(p.z, A.z))
						return true;
					else return false;
				if (!b1 && b2 && b3)
					if (math.same(p.y, A.y) && math.same(p.z, A.z))
						return true;
					else return false;
				/*
				line3d line1 = new line3d(this.A, this.B);
				line3d line2 = new line3d(this.B, p);
				line3d line3 = new line3d(p, this.A);
				if (line1.collinear(line2) || line2.collinear(line3) || line3.collinear(line1))
					return true;
				*/
			}
			return false;
		}
		public bool intersect(line3d line) { return intersect(line, true); }
		public bool intersect(line3d line, bool z)
		{
			if (this.isNull || line.isNull)
				return false;
			if (!z)
				return this.cloneNoZ().intersect(line.cloneNoZ(), true);
			if (this == line)
				return true;
			if (this.rect3d.intersect(line.rect3d))
			{
				if (this.crosses(line, true, true))
					return true;
				if (this.sameStraight(line, true))
				{
					if (this.pointInside(line.A) && line.A != this.A && line.A != this.B)
						return true;
					if (this.pointInside(line.B) && line.B != this.A && line.B != this.B)
						return true;
					if (line.pointInside(this.A) && this.A != line.A && this.A != line.B)
						return true;
					if (line.pointInside(this.B) && this.B != line.A && this.B != line.B)
						return true;
				}
			}
			return false;
		}
		public bool intersect(List<line3d> lines) { return intersect(lines, true); }
		public bool intersect(List<line3d> lines, bool z)
		{
			if (this.isNull)
				return false;
			if (!z)
				return this.cloneNoZ().intersect(lines.Select(zxc => zxc.cloneNoZ()).ToList(), true);
			for (int i = 0; i < lines.Count; i++)
				if (this.intersect(lines[i], true))
					return true;
			return false;
		}
		public bool overlay(line3d line) { return overlay(line, true); }
		public bool overlay(line3d line, bool z)
		{
			if (this.isNull || line.isNull)
				return false;
			if (!z)
				return this.cloneNoZ().overlay(line.cloneNoZ(), true);
			if (this == line)
				return true;
			if (this.rect3d.intersect(line.rect3d))
				if (this.sameStraight(line, true))
				{
					if (this.pointInside(line.A, true) && line.A != this.A && line.A != this.B)
						return true;
					if (this.pointInside(line.B, true) && line.B != this.A && line.B != this.B)
						return true;
					if (line.pointInside(this.A, true) && this.A != line.A && this.A != line.B)
						return true;
					if (line.pointInside(this.B, true) && this.B != line.A && this.B != line.B)
						return true;
				}
			return false;
		}
		public bool overlay(List<line3d> lines) { return overlay(lines, true); }
		public bool overlay(List<line3d> lines, bool z)
		{
			if (this.isNull)
				return false;
			if (!z)
				return this.cloneNoZ().overlay(lines.Select(zxc => zxc.cloneNoZ()).ToList(), true);
			for (int i = 0; i < lines.Count; i++)
				if (this.overlay(lines[i], true))
					return true;
			return false;
		}
		public bool continues(line3d line) { return continues(line, true); }
		public bool continues(line3d line, bool z)
		{
			if (this.isNull || line.isNull)
				return false;
			if (!z)
				return this.cloneNoZ().continues(line.cloneNoZ(), true);
			if (this == line)
				return true;
			if (this.rect3d.intersect(line.rect3d) || this.shareEdgePoint(line, true))
				if (this.sameStraight(line, true))
					return true;
			return false;
		}
		public bool shareEdgePoint(line3d line) { return shareEdgePoint(line, true); }
		public bool shareEdgePoint(line3d line, bool z)
		{
			if (this.isNull || line.isNull)
				return false;
			if (!z)
				return this.cloneNoZ().shareEdgePoint(line.cloneNoZ(), true);
			if (this == line)
				return true;
			if (this.A == line.A || this.A == line.B || this.B == line.A || this.B == line.B)
				return true;
			return false;
		}
		public bool sameStraight(line3d line) { return sameStraight(line, true); }
		public bool sameStraight(line3d line, bool z)
		{
			if (this.isNull || line.isNull)
				return false;
			if (!z)
				return this.cloneNoZ().sameStraight(line.cloneNoZ(), true);
			if (this.collinear(line))
				if (line.endless().pointInside(this.endless().center(), true))
					return true;
			return false;
		}
		public bool sameStraight(List<line3d> lines) { return sameStraight(lines, true); }
		public bool sameStraight(List<line3d> lines, bool z)
		{
			if (this.isNull)
				return false;
			if (!z)
				return this.cloneNoZ().sameStraight(lines.Select(line => line.cloneNoZ()).ToList(), true);

			for (int i = 0; i < lines.Count; i++)
				if (sameStraight(lines[i], true))
					return true;
			return false;
		}
		public bool collinearSurface(poly3d p) { return collinearSurface(this, p); }
		public bool collinearSurface(line3d A, poly3d poly)
		{
			if (A.isNull || poly.isNull)
				return false;
			double[] eq = poly.eq;
			if (eq.Length == 0)
				return false;
			double val1 = eq[0] * A.A.x + eq[1] * A.A.y + eq[2] * A.A.z + eq[3];
			double val2 = eq[0] * A.e.x + eq[1] * A.e.y + eq[2] * A.e.z;
			if (!math.same(val1, 0) && math.same(val2, 0))
				return true;
			return false;
		}
		public bool sameSurface(poly3d p) { return sameSurface(this, p); }
		public bool sameSurface(line3d A, poly3d poly)
		{
			if (A.isNull || poly.isNull)
				return false;
			double[] eq = poly.eq;
			if (eq.Length == 0)
				return false;
			double val1 = eq[0] * A.A.x + eq[1] * A.A.y + eq[2] * A.A.z + eq[3];
			double val2 = eq[0] * A.e.x + eq[1] * A.e.y + eq[2] * A.e.z;
			if (math.same(val1, 0) && math.same(val2, 0))
				return true;
			return false;
		}
		public bool sameSurface(line3d A) { return sameSurface(this, A); }
		public bool sameSurface(line3d A, line3d B)
		{
			if (A.isNull || B.isNull)
				return false;
			return math.same(math.deter(new double[] { B.A.x - A.A.x, B.A.y - A.A.y, B.A.z - A.A.z, A.e.x, A.e.y, A.e.z, B.e.x, B.e.y, B.e.z }), 0);
		}
		public bool collinear(line3d line) { return collinear(this, line); }
		public bool collinear(line3d line1, line3d line2)
		{
			if (!line1.isNull && !line2.isNull)
				if (line1.e == line2.e || line1.e == line2.e * (-1))
					return true;
			return false;
		}
		public point3d center()
		{
			return this.centerpoint.clone();
		}
		public point3d crossingPoint(poly3d poly)
		{
			if (this.isNull || poly.isNull)
				return new point3d();
			if (!math.same(this.e.scalarMult(poly.norm), 0))
			{
				double a = poly.eq[0], b = poly.eq[1], c = poly.eq[2], d = poly.eq[3];
				double ex = this.e.x, ey = this.e.y, ez = this.e.z;
				double x0 = this.A.x, y0 = this.A.y, z0 = this.A.z;
				double t = (-a * x0 - b * y0 - c * z0 - d) / (a * ex + b * ey + c * ez);
				if (math.isNaN(t))
					return new point3d();
				point3d p = this.t(t, true);
				if (p == this.A)
					return this.A;
				if (p == this.B)
					return this.B;
				if (this.rect3d.pointInbound(p) && poly.pointInside(p, true))
					return p;
			}
			return new point3d();
		}
		public point3d crossingPoint(line3d line) { return crossingPoint(line, true); }
		public point3d crossingPoint(line3d line, bool z)
		{
			if (z)
				return crossingPointFunc(line);
			else
				return this.cloneNoZ().crossingPointFunc(line.cloneNoZ());
		}
		private point3d crossingPointFunc(line3d line)
		{
			if (!this.isNull && !line.isNull)
			{
				if (!this.rect3d.intersect(line.rect3d))
					return new point3d();

				if (this.collinear(line))
					return new point3d();

				if (!this.sameSurface(line))
					return new point3d();

				if (line.pointInside(this.A, true))
					return this.A;
				if (line.pointInside(this.B, true))
					return this.B;
				if (this.pointInside(line.A, true))
					return line.A;
				if (this.pointInside(line.B, true))
					return line.B;

				double t0, t1;
				double x0 = this.A.x, y0 = this.A.y, z0 = this.A.z, x1 = line.A.x, y1 = line.A.y, z1 = line.A.z;
				double e0x = this.e.x, e0y = this.e.y, e0z = this.e.z, e1x = line.e.x, e1y = line.e.y, e1z = line.e.z;
				if (!math.same(e1x, 0) && !math.same(e0y - (e1y * e0x / e1x), 0)) // 1->2 , 1
				{
					t0 = (y1 - y0 + (e1y / e1x) * (x0 - x1)) / (e0y - (e1y * e0x / e1x));
					t1 = (x0 - x1 + e0x * t0) / e1x;
					if (!math.isNaN(t0) && !math.isNaN(t1))
					{
						point3d p1 = this.t(t0, true);
						point3d p2 = line.t(t1, true);
						if (p1 == p2 && this.rect3d.pointInbound(p1) && line.rect3d.pointInbound(p2))
							return (p1 + p2) / 2.0;
					}
					return new point3d();
				}
				if (!math.same(e0x, 0) && !math.same(-e1y + (e0y * e1x / e0x), 0)) // 1->2 , 2
				{
					t1 = (y1 - y0 + (e0y / e0x) * (x0 - x1)) / (-e1y + (e0y * e1x / e0x));
					t0 = (x1 - x0 + e1x * t1) / e0x;
					if (!math.isNaN(t0) && !math.isNaN(t1))
					{
						point3d p1 = this.t(t0, true);
						point3d p2 = line.t(t1, true);
						if (p1 == p2 && this.rect3d.pointInbound(p1) && line.rect3d.pointInbound(p2))
							return (p1 + p2) / 2.0;
					}
					return new point3d();
				}
				if (!math.same(e1y, 0) && !math.same(e0x - (e1x * e0y / e1y), 0)) // 2->1 , 1
				{
					t0 = (x1 - x0 + (e1x / e1y) * (y0 - y1)) / (e0x - (e1x * e0y / e1y));
					t1 = (y0 - y1 + e0y * t0) / e1y;
					if (!math.isNaN(t0) && !math.isNaN(t1))
					{
						point3d p1 = this.t(t0, true);
						point3d p2 = line.t(t1, true);
						if (p1 == p2 && this.rect3d.pointInbound(p1) && line.rect3d.pointInbound(p2))
							return (p1 + p2) / 2.0;
					}
					return new point3d();
				}
				if (!math.same(e0y, 0) && !math.same(-e1x + (e0x * e1y / e0y), 0)) // 2->1 , 2
				{
					t1 = (x1 - x0 + (e0x / e0y) * (y0 - y1)) / (-e1x + (e0x * e1y / e0y));
					t0 = (y1 - y0 + e1y * t1) / e0y;
					if (!math.isNaN(t0) && !math.isNaN(t1))
					{
						point3d p1 = this.t(t0, true);
						point3d p2 = line.t(t1, true);
						if (p1 == p2 && this.rect3d.pointInbound(p1) && line.rect3d.pointInbound(p2))
							return (p1 + p2) / 2.0;
					}
					return new point3d();
				}
				if (!math.same(e1y, 0) && !math.same(e0z - (e1z * e0y / e1y), 0)) // 2->3 , 1
				{
					t0 = (z1 - z0 + (e1z / e1y) * (y0 - y1)) / (e0z - (e1z * e0y / e1y));
					t1 = (y0 - y1 + e0y * t0) / e1y;
					if (!math.isNaN(t0) && !math.isNaN(t1))
					{
						point3d p1 = this.t(t0, true);
						point3d p2 = line.t(t1, true);
						if (p1 == p2 && this.rect3d.pointInbound(p1) && line.rect3d.pointInbound(p2))
							return (p1 + p2) / 2.0;
					}
					return new point3d();
				}
				if (!math.same(e0y, 0) && !math.same(-e1z + (e0z * e1y / e0y), 0)) // 2->3 , 2
				{
					t1 = (z1 - z0 + (e0z / e0y) * (y0 - y1)) / (-e1z + (e0z * e1y / e0y));
					t0 = (y1 - y0 + e1y * t1) / e0y;
					if (!math.isNaN(t0) && !math.isNaN(t1))
					{
						point3d p1 = this.t(t0, true);
						point3d p2 = line.t(t1, true);
						if (p1 == p2 && this.rect3d.pointInbound(p1) && line.rect3d.pointInbound(p2))
							return (p1 + p2) / 2.0;
					}
					return new point3d();
				}
				if (!math.same(e1z, 0) && !math.same(e0y - (e1y * e0z / e1z), 0)) // 3->2 , 1
				{
					t0 = (y1 - y0 + (e1y / e1z) * (z0 - z1)) / (e0y - (e1y * e0z / e1z));
					t1 = (z0 - z1 + e0z * t0) / e1z;
					if (!math.isNaN(t0) && !math.isNaN(t1))
					{
						point3d p1 = this.t(t0, true);
						point3d p2 = line.t(t1, true);
						if (p1 == p2 && this.rect3d.pointInbound(p1) && line.rect3d.pointInbound(p2))
							return (p1 + p2) / 2.0;
					}
					return new point3d();
				}
				if (!math.same(e0z, 0) && !math.same(-e1y + (e0y * e1z / e0z), 0)) // 3->2 , 2
				{
					t1 = (y1 - y0 + (e0y / e0z) * (z0 - z1)) / (-e1y + (e0y * e1z / e0z));
					t0 = (z1 - z0 + e1z * t1) / e0z;
					if (!math.isNaN(t0) && !math.isNaN(t1))
					{
						point3d p1 = this.t(t0, true);
						point3d p2 = line.t(t1, true);
						if (p1 == p2 && this.rect3d.pointInbound(p1) && line.rect3d.pointInbound(p2))
							return (p1 + p2) / 2.0;
					}
					return new point3d();
				}
				if (!math.same(e1z, 0) && !math.same(e0x - (e1x * e0z / e1z), 0)) // 3->1 , 1
				{
					t0 = (x1 - x0 + (e1x / e1z) * (z0 - z1)) / (e0x - (e1x * e0z / e1z));
					t1 = (z0 - z1 + e0z * t0) / e1z;
					if (!math.isNaN(t0) && !math.isNaN(t1))
					{
						point3d p1 = this.t(t0, true);
						point3d p2 = line.t(t1, true);
						if (p1 == p2 && this.rect3d.pointInbound(p1) && line.rect3d.pointInbound(p2))
							return (p1 + p2) / 2.0;
					}
					return new point3d();
				}
				if (!math.same(e0z, 0) && !math.same(-e1x + (e0x * e1z / e0z), 0)) // 3->1 , 2
				{
					t1 = (x1 - x0 + (e0x / e0z) * (z0 - z1)) / (-e1x + (e0x * e1z / e0z));
					t0 = (z1 - z0 + e1z * t1) / e0z;
					if (!math.isNaN(t0) && !math.isNaN(t1))
					{
						point3d p1 = this.t(t0, true);
						point3d p2 = line.t(t1, true);
						if (p1 == p2 && this.rect3d.pointInbound(p1) && line.rect3d.pointInbound(p2))
							return (p1 + p2) / 2.0;
					}
					return new point3d();
				}
				if (!math.same(e1x, 0) && !math.same(e0z - (e1z * e0x / e1x), 0)) // 1->3 , 1
				{
					t0 = (z1 - z0 + (e1z / e1x) * (x0 - x1)) / (e0z - (e1z * e0x / e1x));
					t1 = (x0 - x1 + e0x * t0) / e1x;
					if (!math.isNaN(t0) && !math.isNaN(t1))
					{
						point3d p1 = this.t(t0, true);
						point3d p2 = line.t(t1, true);
						if (p1 == p2 && this.rect3d.pointInbound(p1) && line.rect3d.pointInbound(p2))
							return (p1 + p2) / 2.0;
					}
					return new point3d();
				}
				if (!math.same(e0x, 0) && !math.same(-e1z + (e0z * e1x / e0x), 0)) // 1->3 , 2
				{
					t1 = (z1 - z0 + (e0z / e0x) * (x0 - x1)) / (-e1z + (e0z * e1x / e0x));
					t0 = (x1 - x0 + e1x * t1) / e0x;
					if (!math.isNaN(t0) && !math.isNaN(t1))
					{
						point3d p1 = this.t(t0, true);
						point3d p2 = line.t(t1, true);
						if (p1 == p2 && this.rect3d.pointInbound(p1) && line.rect3d.pointInbound(p2))
							return (p1 + p2) / 2.0;
					}
					return new point3d();
				}
			}
			return new point3d();
		}
		public bool crosses(line3d line) { return crosses(line, true, true); }
		public bool crosses(line3d line, bool excludeEnd) { return crosses(line, excludeEnd, true); }
		public bool crosses(line3d line, bool excludeEnd, bool z)
		{
			if (this.isNull || line.isNull)
				return false;
			if (!z)
				return this.cloneNoZ().crosses(line.cloneNoZ(), excludeEnd, true);

			point3d cross = crossingPoint(line, z);
			if (cross.isNull)
				return false;
			if (excludeEnd)
				if (cross == this.A || cross == this.B || cross == line.A || cross == line.B)
					return false;
			return true;
		}
		public bool crosses(List<line3d> lines) { return crosses(lines, true, true); }
		public bool crosses(List<line3d> lines, bool excludeEnd) { return crosses(lines, excludeEnd, true); }
		public bool crosses(List<line3d> lines, bool excludeEnd, bool z)
		{
			if (this.isNull)
				return false;
			if (!z)
				return this.cloneNoZ().crosses(lines.Select(line => line.cloneNoZ()).ToList(), excludeEnd, true);

			for (int i = 0; i < lines.Count; i++)
				if (crosses(lines[i], excludeEnd, true))
					return true;
			return false;
		}
		public bool crosses(poly3d poly) { return crosses(poly, true); }
		public bool crosses(poly3d poly, bool excludeEnd)
		{
			if (this.isNull || poly.isNull)
				return false;

			point3d cross = crossingPoint(poly);
			if (cross.isNull)
				return false;
			if (excludeEnd)
				if (cross == this.A || cross == this.B || cross == poly.A || cross == poly.B || cross == poly.C)
					return false;
			return true;
		}
		public string str()
		{
			if (this.isNull) return "null";
			return $"A: {this.A.str()}\nB: {this.B.str()}\nx = {this.e.x.ToString("F4"),5} * t + {this.A.x.ToString("F2"),7}\ny = {this.e.y.ToString("F4"),5} * t + {this.A.y.ToString("F2"),7}\nz = {this.e.z.ToString("F4"),5} * t + {this.A.z.ToString("F2"),7}";
		}
		public string logpoints()
		{
			if (this.isNull) return "null";
			return $"A: {this.A.str()}\nB: {this.B.str()}";
		}
		public line3d reversed()
		{
			if (this.isNull)
				return new line3d();
			return new line3d(B, A);
		}
		public line3d cloneNoZ()
		{
			if (this.isNull)
				return new line3d();
			point3d newA = new point3d(A.x, A.y);
			point3d newB = new point3d(B.x, B.y);
			return new line3d(newA, newB);
		}
		public line3d clone()
		{
			if (this.isNull)
				return new line3d();
			return new line3d() { A = this.A, B = this.B, e = this.e, isNull = this.isNull };
		}
		public line3d endless()
		{
			if (this.isNull)
				return new line3d();
			double maxX, maxY, maxZ, minX, minY, minZ;
			double maxVal = short.MaxValue; double minVal = short.MinValue;
			double maxE = Math.Max(Math.Max(Math.Abs(this.e.x), Math.Abs(this.e.y)), Math.Abs(this.e.z));
			if (math.same(maxE, Math.Abs(this.e.x)))
			{
				double x1, y1, z1, x2, y2, z2, t;

				x1 = maxVal;
				t = (x1 - A.x) / this.e.x;
				y1 = e.y * t + A.y;
				z1 = e.z * t + A.z;

				x2 = minVal;
				t = (x2 - A.x) / this.e.x;
				y2 = e.y * t + A.y;
				z2 = e.z * t + A.z;

				maxX = x1; minX = x2;
				maxY = y1; minY = y2;
				maxZ = z1; minZ = z2;
			}
			else if (math.same(maxE, Math.Abs(this.e.y)))
			{
				double x1, y1, z1, x2, y2, z2, t;

				y1 = maxVal;
				t = (y1 - A.y) / this.e.y;
				x1 = e.x * t + A.x;
				z1 = e.z * t + A.z;

				y2 = minVal;
				t = (y2 - A.y) / this.e.y;
				x2 = e.x * t + A.x;
				z2 = e.z * t + A.z;

				maxX = x1; minX = x2;
				maxY = y1; minY = y2;
				maxZ = z1; minZ = z2;
			}
			else
			{
				double x1, y1, z1, x2, y2, z2, t;

				z1 = maxVal;
				t = (z1 - A.z) / this.e.z;
				x1 = e.x * t + A.x;
				y1 = e.y * t + A.y;

				z2 = minVal;
				t = (z2 - A.z) / this.e.z;
				x2 = e.x * t + A.x;
				y2 = e.y * t + A.y;

				maxX = x1; minX = x2;
				maxY = y1; minY = y2;
				maxZ = z1; minZ = z2;
			}
			return new line3d(new point3d(minX, minY, minZ), new point3d(maxX, maxY, maxZ));
		}
		public static bool operator ==(line3d A, line3d B)
		{
			if (A.isNull && B.isNull)
				return true;
			if (!A.isNull && !B.isNull)
				if ((A.A == B.A && A.B == B.B) || (A.A == B.B && A.B == B.A))
					return true;
			return false;
		}
		public static bool operator !=(line3d A, line3d B)
		{
			return !(A == B);
		}
		public override bool Equals(object? obj)
		{
			if (obj == null)
				return false;
			if (!(obj is line3d))
				return false;
			return this == (line3d)obj;
		}
		public override int GetHashCode()
		{
			return Tuple.Create(A, B, e, isNull).GetHashCode();
		}
		private void toNull()
		{
			this.A = new point3d();
			this.B = new point3d();
			this.e = new point3d();
			this.centerpoint = new point3d();
			this.rect3d = new rect3d();
			this.isNull = true;
		}
	}
	public struct poly3d
	{
		public point3d A { get; set; }
		public point3d B { get; set; }
		public point3d C { get; set; }
		public double[] eq { get; set; }
		public point3d norm { get; set; }
		public double area { get; set; }
		public double areaz { get; set; }
		public Rectangle rect { get; set; }
		public rect3d rect3d { get; set; }
		private point3d centerpoint { get; set; }
		private List<Point> allpoints { get; set; }
		private List<line3d> alllines { get; set; }
		public bool isNull { get; set; }
		public poly3d() { toNull(); }
		public poly3d(point3d A, point3d B, point3d C)
		{
			if (A != B && A != C && !A.isNull && !B.isNull && !C.isNull)
			{
				this.A = A;
				this.B = B;
				this.C = C;
				double A1 = (B.y - A.y) * (C.z - A.z) - (C.y - A.y) * (B.z - A.z);
				double B1 = (C.x - A.x) * (B.z - A.z) - (B.x - A.x) * (C.z - A.z);
				double C1 = (B.x - A.x) * (C.y - A.y) - (C.x - A.x) * (B.y - A.y);
				double D1 =
					A.x * ((C.y - A.y) * (B.z - A.z) - (B.y - A.y) * (C.z - A.z)) +
					A.y * ((B.x - A.x) * (C.z - A.z) - (C.x - A.x) * (B.z - A.z)) +
					A.z * ((C.x - A.x) * (B.y - A.y) - (B.x - A.x) * (C.y - A.y));

				double len1 = Math.Sqrt(A1 * A1 + B1 * B1 + C1 * C1);
				if (C1 < 0)
					len1 *= -1;
				A1 /= len1;
				B1 /= len1;
				C1 /= len1;
				D1 /= len1;
				if (math.isNaN(A1) || math.isNaN(B1) || math.isNaN(C1) || math.isNaN(D1))
				{
					toNull();
					return;
				}
				this.eq = new double[] { A1, B1, C1, D1 };
				this.norm = new point3d(eq[0], eq[1], eq[2]);
				if (math.same(this.norm.z, 0))
				{
					toNull();
					return;
				}
				double a, b, c, p;
				a = Math.Sqrt((A.x - B.x) * (A.x - B.x) + (A.y - B.y) * (A.y - B.y));
				b = Math.Sqrt((B.x - C.x) * (B.x - C.x) + (B.y - C.y) * (B.y - C.y));
				c = Math.Sqrt((C.x - A.x) * (C.x - A.x) + (C.y - A.y) * (C.y - A.y));
				p = (a + b + c) / 2.0;
				this.area = Math.Sqrt(p * (p - a) * (p - b) * (p - c));
				if (math.same(this.area, 0))
				{
					toNull();
					return;
				}
				a = Math.Sqrt((A.x - B.x) * (A.x - B.x) + (A.y - B.y) * (A.y - B.y) + (A.z - B.z) * (A.z - B.z));
				b = Math.Sqrt((B.x - C.x) * (B.x - C.x) + (B.y - C.y) * (B.y - C.y) + (B.z - C.z) * (B.z - C.z));
				c = Math.Sqrt((C.x - A.x) * (C.x - A.x) + (C.y - A.y) * (C.y - A.y) + (C.z - A.z) * (C.z - A.z));
				p = (a + b + c) / 2.0;
				this.areaz = Math.Sqrt(p * (p - a) * (p - b) * (p - c));
				setCenter();
				setBounds();
				setLines();
				this.allpoints = new List<Point>();
				this.isNull = false;
			}
			else
				toNull();
		}
		private void setCenter()
		{
			this.centerpoint = (A + B + C) / 3.0;
		}
		private void setBounds()
		{
			double maxX = Math.Max(Math.Max(A.x, B.x), C.x);
			double minX = Math.Min(Math.Min(A.x, B.x), C.x);
			double maxY = Math.Max(Math.Max(A.y, B.y), C.y);
			double minY = Math.Min(Math.Min(A.y, B.y), C.y);
			double maxZ = Math.Max(Math.Max(A.z, B.z), C.z);
			double minZ = Math.Min(Math.Min(A.z, B.z), C.z);
			this.rect3d = new rect3d(minX, minY, minZ, maxX, maxY, maxZ);
			int left = (int)Math.Ceiling(minX); int right = (int)Math.Floor(maxX); int width = right - left;
			int top = (int)Math.Ceiling(minY); int bottom = (int)Math.Floor(maxY); int height = bottom - top;
			if (width < 0 || height < 0)
				this.rect = new Rectangle();
			else
				this.rect = new Rectangle(left, top, width, height);
		}
		private void setLines()
		{
			this.alllines = new List<line3d>() { new line3d(this.A, this.B), new line3d(this.B, this.C), new line3d(this.C, this.A) };
		}
		public bool shareLine(poly3d poly) { return shareLine(poly, true); }
		public bool shareLine(poly3d poly, bool z)
		{
			if (this.isNull || poly.isNull)
				return false;
			if (!z)
				return this.cloneNoZ().shareLine(poly.cloneNoZ(), true);
			List<line3d> lines1 = this.lines();
			List<line3d> lines2 = poly.lines();
			for (int i = 0; i < 3; i++)
				for (int j = 0; j < 3; j++)
					if (lines1[i].overlay(lines2[j], true))
						return true;
			return false;
		}
		public bool sameSurface(poly3d poly)
		{
			if (this.isNull || poly.isNull)
				return false;
			double[] eq1 = this.eq;
			double[] eq2 = poly.eq;
			if (eq1.Length == 0 || eq2.Length == 0 || eq1.Length != eq2.Length)
				return false;
			bool sameP = true, sameM = true;
			for (int i = 0; i < eq1.Length; i++)
				if (!math.same(eq1[i], eq2[i]))
				{ sameP = false; break; }
			for (int i = 0; i < eq1.Length; i++)
				if (!math.same(eq1[i], eq2[i] * (-1)))
				{ sameM = false; break; }
			if (!sameP && !sameM)
				return false;
			return true;
		}
		public bool pointOnSurface(point3d p)
		{
			if (this.isNull || p.isNull)
				return false;
			return math.same(eq[0] * p.x + eq[1] * p.y + eq[2] * p.z + eq[3], 0);
		}
		public point3d surfacePoint(point3d p)
		{
			if (this.isNull || p.isNull)
				return new point3d();
			if (eq.Length == 0)
				return new point3d();
			double z;
			if (!math.same(eq[2], 0))
				z = (-1.0 / eq[2]) * (eq[0] * p.x + eq[1] * p.y + eq[3]);
			else
				z = A.z;
			return new point3d(p.x, p.y, z);
		}
		public List<point3d> surfacePoints(List<point3d> ps)
		{
			if (this.isNull)
				return Enumerable.Repeat(new point3d(), ps.Count).ToList();
			if (eq.Length == 0)
				return Enumerable.Repeat(new point3d(), ps.Count).ToList();
			List<point3d> ret = ps.ToList();
			if (!math.same(eq[2], 0))
				for (int i = 0; i < ret.Count; i++)
				{
					double z = (-1.0 / eq[2]) * (eq[0] * ret[i].x + eq[1] * ret[i].y + eq[3]);
					ret[i] = new point3d(ret[i].x, ret[i].y, z);
				}
			else
				for (int i = 0; i < ret.Count; i++)
				{
					ret[i] = new point3d(ret[i].x, ret[i].y, A.z);
				}
			return ret;
		}
		public bool pointInbound(point3d p) { return pointInbound(p, true); }
		public bool pointInbound(point3d p, bool z)
		{
			if (this.isNull || p.isNull)
				return false;
			if (!z)
				return this.cloneNoZ().pointInbound(p.cloneNoZ(), true);
			if (p == A || p == B || p == C)
				return true;
			return this.rect3d.pointInbound(p);
		}
		public List<Point> allPoints()
		{
			if (this.isNull || this.rect.IsEmpty) return new List<Point>();
			if (this.allpoints.Count != 0) return new List<Point>(this.allpoints);
			List<Point> points = new List<Point>();
			poly3d noZ = this.cloneNoZ();
			List<line3d> lines = noZ.lines();
			if (rect.Width < rect.Height)
			{
				for (int i = rect.Left; i <= rect.Right; i++)
				{
					line3d vert = new line3d(new point3d(i, int.MaxValue), new point3d(i, int.MinValue));
					List<double> ys = new List<double>(3);
					for (int j = 0; j < 3; j++)
					{
						point3d p = lines[j].crossingPoint(vert, true);
						if (!p.isNull)
							ys.Add(p.y);
					}
					if (ys.Count > 1)
					{
						int min = (int)Math.Max(Math.Round(ys.Min()), rect.Top);
						int max = (int)Math.Min(Math.Round(ys.Max()), rect.Bottom);
						for (int j = min; j <= max; j++)
						{
							points.Add(new Point(i, j));
						}
					}
				}
			}
			else
			{
				for (int j = rect.Top; j <= rect.Bottom; j++)
				{
					line3d hor = new line3d(new point3d(int.MaxValue, j), new point3d(int.MinValue, j));
					List<double> xs = new List<double>(3);
					for (int i = 0; i < 3; i++)
					{
						point3d p = lines[i].crossingPoint(hor, true);
						if (!p.isNull)
							xs.Add(p.x);
					}
					if (xs.Count > 1)
					{
						int min = (int)Math.Max(Math.Round(xs.Min()), rect.Left);
						int max = (int)Math.Min(Math.Round(xs.Max()), rect.Right);
						for (int i = min; i <= max; i++)
						{
							points.Add(new Point(i, j));
						}
					}
				}
			}
			this.allpoints = points;
			return new List<Point>(this.allpoints);
		}
		public Point randpoint()
		{
			if (this.isNull || math.same(this.area, 0))
				return new Point();
			List<Point> allpoints = allPoints();
			if (allpoints.Count == 0) return new Point();
			return allpoints[math.rnd.Next(allpoints.Count)];
		}
		public bool pointInside(point3d p) { return pointInside(p, true); }
		public bool pointInside(point3d p, bool z)
		{
			if (this.isNull || p.isNull)
				return false;
			if (!z)
				return this.cloneNoZ().pointInside(p.cloneNoZ(), true);
			if (!this.rect3d.pointInbound(p))
				return false;
			if (p == this.A || p == this.B || p == this.C)
				return false;
			if (!this.pointOnSurface(p))
				return false;
			//double mink = Math.Min(1.0 / Math.Sqrt(128 * this.area), 0.01), maxk = 1.0 - mink, mins = mink * 2.0, maxs = 1.0 - mins;
			double mink = 0.001 / Math.Sqrt(2 * this.area), maxk = 1.0 - mink, mins = mink * 2.0, maxs = 1.0 - mins;
			//double mink = 1.0 / Math.Sqrt(8 * this.area), maxk = 1.0 - mink, mins = mink * 2.0, maxs = 1.0 - mins;
			//double mink = 0.001, maxk = 1.0 - mink, mins = mink * 2.0, maxs = 1.0 - mins;
			for (int i = 0; i < 3; i++)
			{
				point3d v1 = new point3d(), v2 = new point3d(), pv = new point3d();
				if (i == 0) { v1 = this.B - this.A; v2 = this.C - this.A; pv = p - this.A; }
				if (i == 1) { v1 = this.A - this.B; v2 = this.C - this.B; pv = p - this.B; }
				if (i == 2) { v1 = this.A - this.C; v2 = this.B - this.C; pv = p - this.C; }
				double v1x = v1.x, v1y = v1.y, v1z = v1.z, v2x = v2.x, v2y = v2.y, v2z = v2.z;
				double px = pv.x, py = pv.y, pz = pv.z;
				double a, b;
				if (!math.same(v1x, 0) && !math.same(v2y - (v1y * v2x / v1x), 0)) // 1 -> 2 , 1
				{
					b = (py - (v1y / v1x) * px) / (v2y - (v1y * v2x / v1x));
					a = (px - v2x * b) / v1x;
					if (a > mink && a < maxk && b > mink && b < maxk && (a + b) > mins && (a + b) < maxs)
						return true;
					else return false;
				}
				if (!math.same(v2x, 0) && !math.same(v1y - (v2y * v1x / v2x), 0)) // 1 -> 2 , 2
				{
					a = (py - (v2y / v2x) * px) / (v1y - (v2y * v1x / v2x));
					b = (px - v1x * a) / v2x;
					if (a > mink && a < maxk && b > mink && b < maxk && (a + b) > mins && (a + b) < maxs)
						return true;
					else return false;
				}
				if (!math.same(v1y, 0) && !math.same(v2x - (v1x * v2y / v1y), 0)) // 2 -> 1 , 1
				{
					b = (px - (v1x / v1y) * py) / (v2x - (v1x * v2y / v1y));
					a = (py - v2y * b) / v1y;
					if (a > mink && a < maxk && b > mink && b < maxk && (a + b) > mins && (a + b) < maxs)
						return true;
					else return false;
				}
				if (!math.same(v2y, 0) && !math.same(v1x - (v2x * v1y / v2y), 0)) // 2 -> 1 , 2
				{
					a = (px - (v2x / v2y) * py) / (v1x - (v2x * v1y / v2y));
					b = (py - v1y * a) / v2y;
					if (a > mink && a < maxk && b > mink && b < maxk && (a + b) > mins && (a + b) < maxs)
						return true;
					else return false;
				}
				if (!math.same(v1y, 0) && !math.same(v2z - (v1z * v2y / v1y), 0)) // 2 -> 3 , 1
				{
					b = (pz - (v1z / v1y) * py) / (v2z - (v1z * v2y / v1y));
					a = (py - v2y * b) / v1y;
					if (a > mink && a < maxk && b > mink && b < maxk && (a + b) > mins && (a + b) < maxs)
						return true;
					else return false;
				}
				if (!math.same(v2y, 0) && !math.same(v1z - (v2z * v1y / v2y), 0)) // 2 -> 3 , 2
				{
					a = (pz - (v2z / v2y) * py) / (v1z - (v2z * v1y / v2y));
					b = (py - v1y * a) / v2y;
					if (a > mink && a < maxk && b > mink && b < maxk && (a + b) > mins && (a + b) < maxs)
						return true;
					else return false;
				}
				if (!math.same(v1z, 0) && !math.same(v2y - (v1y * v2z / v1z), 0)) // 3 -> 2 , 1
				{
					b = (py - (v1y / v1z) * pz) / (v2y - (v1y * v2z / v1z));
					a = (pz - v2z * b) / v1z;
					if (a > mink && a < maxk && b > mink && b < maxk && (a + b) > mins && (a + b) < maxs)
						return true;
					else return false;
				}
				if (!math.same(v2z, 0) && !math.same(v1y - (v2y * v1z / v2z), 0)) // 3 -> 2 , 2
				{
					a = (py - (v2y / v2z) * pz) / (v1y - (v2y * v1z / v2z));
					b = (pz - v1z * a) / v2z;
					if (a > mink && a < maxk && b > mink && b < maxk && (a + b) > mins && (a + b) < maxs)
						return true;
					else return false;
				}
				if (!math.same(v1z, 0) && !math.same(v2x - (v1x * v2z / v1z), 0)) // 3 -> 1 , 1
				{
					b = (px - (v1x / v1z) * pz) / (v2x - (v1x * v2z / v1z));
					a = (pz - v2z * b) / v1z;
					if (a > mink && a < maxk && b > mink && b < maxk && (a + b) > mins && (a + b) < maxs)
						return true;
					else return false;
				}
				if (!math.same(v2z, 0) && !math.same(v1x - (v2x * v1z / v2z), 0)) // 3 -> 1 , 2
				{
					a = (px - (v2x / v2z) * pz) / (v1x - (v2x * v1z / v2z));
					b = (pz - v1z * a) / v2z;
					if (a > mink && a < maxk && b > mink && b < maxk && (a + b) > mins && (a + b) < maxs)
						return true;
					else return false;
				}
				if (!math.same(v1x, 0) && !math.same(v2z - (v1z * v2x / v1x), 0)) // 1 -> 3 , 1
				{
					b = (pz - (v1z / v1x) * px) / (v2z - (v1z * v2x / v1x));
					a = (px - v2x * b) / v1x;
					if (a > mink && a < maxk && b > mink && b < maxk && (a + b) > mins && (a + b) < maxs)
						return true;
					else return false;
				}
				if (!math.same(v2x, 0) && !math.same(v1z - (v2z * v1x / v2x), 0)) // 1 -> 3 , 2
				{
					a = (pz - (v2z / v2x) * px) / (v1z - (v2z * v1x / v2x));
					b = (px - v1x * a) / v2x;
					if (a > mink && a < maxk && b > mink && b < maxk && (a + b) > mins && (a + b) < maxs)
						return true;
					else return false;
				}
			}
			return false;
		}
		public bool intersect(List<poly3d> polys) { return intersect(polys, true); }
		public bool intersect(List<poly3d> polys, bool z)
		{
			if (this.isNull)
				return false;
			if (!z)
				return this.cloneNoZ().intersect(polys.Select(poly => poly.cloneNoZ()).ToList(), true);

			for (int i = 0; i < polys.Count; i++)
				if (this.intersect(polys[i], true))
					return true;
			return false;
		}
		public bool intersect(poly3d poly) { return intersect(poly, true); }
		public bool intersect(poly3d poly, bool z)
		{
			if (this.isNull || poly.isNull)
				return false;
			if (!z)
				return this.cloneNoZ().intersect(poly.cloneNoZ(), true);
			if (this == poly)
				return true;
			if (!this.rect3d.intersect(poly.rect3d))
				return false;

			List<line3d> lines1 = this.lines();
			List<line3d> lines2 = poly.lines();

			if (!this.sameSurface(poly))
			{
				for (int i = 0; i < lines1.Count; i++)
					if (lines1[i].crosses(poly, true))
						return true;
				for (int i = 0; i < lines2.Count; i++)
					if (lines2[i].crosses(this, true))
						return true;
				return false;
			}

			for (int i = 0; i < lines1.Count; i++)
				if (lines1[i].crosses(lines2, true, true))
					return true;

			List<point3d> points1 = new List<point3d>() { this.A, this.B, this.C, this.center(), lines1[0].center(), lines1[1].center(), lines1[2].center() };
			List<point3d> points2 = new List<point3d>() { poly.A, poly.B, poly.C, poly.center(), lines2[0].center(), lines2[1].center(), lines2[2].center() };
			for (int i = 0; i < points1.Count; i++)
				if (poly.pointInside(points1[i], true))
					return true;
			for (int i = 0; i < points2.Count; i++)
				if (this.pointInside(points2[i], true))
					return true;

			return false;
		}
		public point3d center()
		{
			return this.centerpoint.clone();
		}
		public List<poly3d> summarize(List<poly3d> polys)
		{
			for (int i = 0; i < polys.Count; i++)
				if (polys[i].isNull)
				{ polys.RemoveAt(i); i--; }

			for (int i = 0; i < polys.Count; i++)
				for (int j = i + 1; j < polys.Count; j++)
					if (polys[i] == polys[j])
					{ polys.RemoveAt(j); j--; }

			if (polys.Count == 0)
				return new List<poly3d>();

			List<poly3d> polysnoZ = polys.Select(poly => poly.cloneNoZ()).ToList();
			List<List<poly3d>> allpolys = new List<List<poly3d>>(polys.Count);

			List<List<int>> cutters = new List<List<int>>(polys.Count);
			List<line3d> splitters = new List<line3d>();
			for (int i = 0; i < polys.Count; i++)
			{
				cutters.Add(new List<int>());
				splitters.AddRange(polysnoZ[i].lines());
				for (int j = i + 1; j < polys.Count; j++)
					if (polysnoZ[i].intersect(polysnoZ[j], true))
					{
						cutters[i].Add(j);
						splitters.Add(new line3d(polys[i], polys[j]).cloneNoZ());
					}
			}
			for (int i = 1; i < cutters.Count; i++)
				for (int j = i - 1; j >= 0; j--)
					if (cutters[j].Contains(i))
						cutters[i].Add(j);

			for (int i = 0; i < polys.Count; i++)
				allpolys.Add(polysnoZ[i].split(splitters, true));

			for (int i = 0; i < allpolys.Count; i++)
				for (int j = 0; j < allpolys[i].Count; j++)
					for (int t = 0; t < cutters[i].Count; t++)
					{
						int u = cutters[i][t];
						for (int v = 0; v < allpolys[u].Count; v++)
							if (allpolys[i][j].intersect(allpolys[u][v], true))
							{
								point3d center = allpolys[i][j].center();
								double thisZ = polys[i].surfacePoint(center).z;
								double polyZ = polys[u].surfacePoint(center).z;
								if (thisZ > polyZ || math.same(thisZ, polyZ))
								{ allpolys[u].RemoveAt(v); v--; }
							}
					}

			List<List<poly3d>> planes = new List<List<poly3d>>();
			List<int> eqIndex = new List<int>();
			List<int> reserved = new List<int>();
			for (int i = 0; i < polys.Count; i++)
			{
				if (!reserved.Contains(i))
				{
					eqIndex.Add(i);
					List<int> ids = new List<int>() { i };
					for (int j = i + 1; j < polys.Count; j++)
						if (!reserved.Contains(j))
							if (polys[i].sameSurface(polys[j]))
								ids.Add(j);
					reserved.AddRange(ids);
					List<poly3d> toadd = new List<poly3d>();
					for (int j = 0; j < ids.Count; j++)
						for (int z = 0; z < allpolys[ids[j]].Count; z++)
							toadd.Add(allpolys[ids[j]][z]);
					planes.Add(toadd);
				}
			}
			switch (calculationSettings.simplificationMethod)
			{
				case 0:
					for (int i = 0; i < planes.Count; i++)
						for (int j = 0; j < planes[i].Count; j++)
							if (planes[i][j].isNull)
							{ planes[i].RemoveAt(j); j--; }
					break;
				case 1:
					for (int i = 0; i < planes.Count; i++)
						planes[i] = simplifyRandom(planes[i]);
					break;
				case 2:
					for (int i = 0; i < planes.Count; i++)
						planes[i] = simplifyRandom(planes[i]);
					for (int i = 0; i < planes.Count; i++)
						planes[i] = simplify(planes[i]);
					for (int i = 0; i < planes.Count; i++)
						planes[i] = simplifyRandom(planes[i]);
					break;
				default:
					for (int i = 0; i < planes.Count; i++)
						planes[i] = simplifyRandom(planes[i]);
					break;
			}

			for (int i = 0; i < planes.Count; i++)
			{
				List<point3d> ps = polys[eqIndex[i]].surfacePoints(planes[i].SelectMany(poly => new List<point3d>() { poly.A, poly.B, poly.C }).ToList());
				for (int j = 0; j < planes[i].Count; j++)
				{
					int c = j * 3;
					planes[i][j] = new poly3d(ps[c], ps[c + 1], ps[c + 2]);
				}
			}
			return planes.SelectMany(zxc => zxc.Where(poly => !poly.isNull)).ToList();
			//return planes.SelectMany(zxc => zxc).ToList();
		}
		public List<poly3d> add(List<poly3d> additions)
		{
			return summarize(additions.Prepend(this).ToList());
		}
		public List<poly3d> add(poly3d sample, List<poly3d> additions)
		{
			return summarize(additions.Prepend(sample).ToList());
		}
		public List<poly3d> add(poly3d addition)
		{
			return summarize(new List<poly3d>() { this, addition });
		}
		public List<poly3d> add(poly3d sample, poly3d addition)
		{
			return summarize(new List<poly3d>() { sample, addition });
		}
		public List<poly3d> cut(List<poly3d> cutters)
		{
			if (this.isNull)
				return new List<poly3d>();

			for (int i = 0; i < cutters.Count; i++)
				if (cutters[i].isNull)
				{ cutters.RemoveAt(i); i--; }

			for (int i = 0; i < cutters.Count; i++)
				for (int j = i + 1; j < cutters.Count; j++)
					if (cutters[i] == cutters[j])
					{ cutters.RemoveAt(j); j--; }

			if (cutters.Count == 0)
				return new List<poly3d>() { this };

			poly3d thisnoZ = this.cloneNoZ();
			List<poly3d> cuttersnoZ = cutters.Select(zxc => zxc.cloneNoZ()).ToList();
			for (int i = 0; i < cuttersnoZ.Count; i++)
				if (!cuttersnoZ[i].intersect(thisnoZ, true))
				{ cuttersnoZ.RemoveAt(i); cutters.RemoveAt(i); i--; }
			if (cutters.Count == 0)
				return new List<poly3d>() { this };

			List<line3d> splitters = new List<line3d>();
			for (int i = 0; i < cutters.Count; i++)
			{
				splitters.AddRange(cuttersnoZ[i].lines());
				splitters.Add(new line3d(this, cutters[i]).cloneNoZ());
			}

			List<poly3d> allpolys = thisnoZ.split(splitters, true);
			for (int i = 0; i < allpolys.Count; i++)
				for (int j = 0; j < cuttersnoZ.Count; j++)
					if (allpolys[i].intersect(cuttersnoZ[j], true))
					{
						point3d center = allpolys[i].center();
						double thisZ = this.surfacePoint(center).z;
						double polyZ = cutters[j].surfacePoint(center).z;
						if (polyZ > thisZ || math.same(polyZ, thisZ))
						{ allpolys.RemoveAt(i); i--; break; }
					}

			switch (calculationSettings.simplificationMethod)
			{
				case 0:
					for (int i = 0; i < allpolys.Count; i++)
						if (allpolys[i].isNull)
						{ allpolys.RemoveAt(i); i--; }
					break;
				case 1:
					allpolys = simplifyRandom(allpolys);
					break;
				case 2:
					allpolys = simplifyRandom(allpolys);
					allpolys = simplify(allpolys);
					allpolys = simplifyRandom(allpolys);
					break;
				default:
					allpolys = simplifyRandom(allpolys);
					break;
			}

			List<point3d> ps = this.surfacePoints(allpolys.SelectMany(poly => new List<point3d>() { poly.A, poly.B, poly.C }).ToList());
			for (int i = 0; i < allpolys.Count; i++)
			{
				int c = i * 3;
				allpolys[i] = new poly3d(ps[c], ps[c + 1], ps[c + 2]);
			}
			return allpolys.Where(poly => !poly.isNull).ToList();
			//return allpolys;
		}
		public List<poly3d> cut(poly3d sample, List<poly3d> cutters)
		{
			return sample.cut(cutters);
		}
		public List<poly3d> cut(poly3d cutter)
		{
			return this.cut(new List<poly3d>() { cutter });
		}
		public List<poly3d> cut(poly3d sample, poly3d cutter)
		{
			return sample.cut(cutter);
		}
		public List<poly3d> split(List<line3d> lines) { return split(lines, true); }
		public List<poly3d> split(List<line3d> lines, bool z)
		{
			if (this.isNull)
				return new List<poly3d>();
			if (lines.Count == 0)
				return new List<poly3d>() { this };
			if (!z)
				return this.cloneNoZ().split(lines.Select(line => line.cloneNoZ()).ToList(), true);

			List<poly3d> polys = new List<poly3d>() { this };
			for (int i = 0; i < lines.Count; i++)
			{
				for (int j = 0; j < polys.Count; j++)
				{
					List<poly3d> newpolys = polys[j].split(lines[i], true);
					if (newpolys.Count == 1)
						if (newpolys[0] == polys[j])
							continue;
					polys.RemoveAt(j);
					polys.InsertRange(j, newpolys);
					j += newpolys.Count - 1;
				}
			}
			return polys;
		}
		public List<poly3d> split(poly3d sample, List<line3d> lines) { return split(sample, lines, true); }
		public List<poly3d> split(poly3d sample, List<line3d> lines, bool z)
		{
			return sample.split(lines, z);
		}
		public List<poly3d> split(line3d line) { return split(line, true); }
		public List<poly3d> split(line3d line, bool z)
		{
			if (this.isNull)
				return new List<poly3d>();
			if (line.isNull)
				return new List<poly3d>() { this };
			if (!z)
				return this.cloneNoZ().split(line.cloneNoZ(), true);

			if (!line.sameSurface(this))
			{
				List<point3d> ps = this.surfacePoints(this.cloneNoZ().split(line.cloneNoZ(), true).Where(poly => !poly.isNull).SelectMany(poly => new List<point3d>() { poly.A, poly.B, poly.C }).ToList());
				if (ps.Count == 0)
					return new List<poly3d>();
				List<poly3d> ret = new List<poly3d>(ps.Count / 3);
				for (int i = 0; i < ps.Count / 3; i++)
				{
					int c = i * 3;
					ret.Add(new poly3d(ps[c], ps[c + 1], ps[c + 2]));
				}
				return ret;
			}

			if (!this.rect3d.intersect(line.rect3d))
				return new List<poly3d>() { this };

			List<line3d> lines = this.lines();
			if (!line.crosses(lines, false, true) && !this.pointInside(line.A, true) && !this.pointInside(line.B, true) && !this.pointInside(line.center(), true))
				return new List<poly3d>() { this };

			line = line.endless();

			if (line.sameStraight(lines, true))
				return new List<poly3d>() { this };
			List<point3d> crosses = new List<point3d>(3);
			List<int> indexes = new List<int>(3);
			for (int i = 0; i < 3; i++)
			{
				point3d cross = line.crossingPoint(lines[i], true);
				if (!cross.isNull && !cross.inlist(crosses))
				{
					crosses.Add(cross);
					indexes.Add(i);
				}
			}
			if (crosses.Count != 2)
				return new List<poly3d>() { this };
			bool c0 = indexes.Contains(0);
			bool c1 = indexes.Contains(1);
			bool c2 = indexes.Contains(2);
			if (c0 && c1)
			{
				return new List<poly3d>()
				{
					new poly3d(crosses[0], this.B, crosses[1]),
					new poly3d(this.A, crosses[0], this.C),
					new poly3d(crosses[0], crosses[1], this.C),
				}.Where(poly => !poly.isNull).ToList();
			}
			if (c0 && c2)
			{
				return new List<poly3d>()
				{
					new poly3d(this.A, crosses[0], crosses[1]),
					new poly3d(crosses[0], this.B, this.C),
					new poly3d(crosses[0], this.C, crosses[1]),
				}.Where(poly => !poly.isNull).ToList();
			}
			if (c1 && c2)
			{
				return new List<poly3d>()
				{
					new poly3d(crosses[1], crosses[0], this.C),
					new poly3d(this.A, this.B, crosses[0]),
					new poly3d(this.A, crosses[0], crosses[1]),
				}.Where(poly => !poly.isNull).ToList();
			}
			return new List<poly3d>() { this };
		}
		public List<poly3d> split(poly3d sample, line3d line) { return split(sample, line, true); }
		public List<poly3d> split(poly3d sample, line3d line, bool z)
		{
			return sample.split(line, z);
		}
		public List<poly3d> polysFromPoints(List<point3d> points)
		{
			for (int i = 0; i < points.Count; i++)
				if (points[i].isNull || points[i] == points[(i + 1) % points.Count])
				{ points.RemoveAt(i); i--; }

			for (int i = 0; i < points.Count; i++)
			{
				int j = (i + 1) % points.Count;
				int k = (i + 2) % points.Count;
				if (new line3d(points[i], points[j]).collinear(new line3d(points[j], points[k])))
				{ points.RemoveAt(j); i--; }
			}
			if (points.Count > 2)
			{
				poly3d sample = new poly3d(points[0], points[1], points[2]);
				point3d norm = sample.norm;
				if (sample.isNull || norm.isNull || math.same(norm.z, 0) || sample.eq.Length == 0)
					return new List<poly3d>();
				points = sample.surfacePoints(points);
				List<line3d> lines = new List<line3d>(points.Count);
				for (int i = 0; i < points.Count; i++)
					lines.Add(new line3d(points[i], points[(i + 1) % points.Count]));

				for (int i = 0; i < lines.Count; i++)
				{
					for (int j = i + 1; j < lines.Count; j++)
					{
						if (lines[i].crosses(lines[j], true, true))
						{
							Console.WriteLine("Shape crosses itself!");
							point3d cross = lines[i].crossingPoint(lines[j], true);
							List<point3d> p1 = new List<point3d>((i + 1) + 1 + (points.Count - 1 - j));
							List<point3d> p2 = new List<point3d>((j - i) + 1);
							for (int k = 0; k <= i; k++)
								p1.Add(points[k]);
							p1.Add(cross);
							for (int k = j + 1; k <= points.Count - 1; k++)
								p1.Add(points[k]);
							for (int k = i + 1; k <= j; k++)
								p2.Add(points[k]);
							p2.Add(cross);
							return polysFromPoints(p1).Concat(polysFromPoints(p2)).ToList();
						}
					}
				}
				bool? clockwise = math.clockwiseDirection(points, norm);
				if (clockwise == null)
					return new List<poly3d>();
				List<poly3d> polys = new List<poly3d>();
				for (int i = 0; i < points.Count && points.Count >= 3; i++)
				{
					if (points.Count == 3)
					{
						poly3d poly = new poly3d(points[0], points[1], points[2]);
						if (!poly.isNull)
							polys.Add(poly);
						break;
					}
					int l = i == 0 ? points.Count - 1 : i - 1;
					int r = i == points.Count - 1 ? 0 : i + 1;
					double angle = math.angle(points[l], points[i], points[r], clockwise, norm);
					if (math.same(angle, 180.0) || math.same(angle, 0.0) || math.same(angle, 360.0))
					{
						line3d newline = new line3d(points[l], points[r]);
						if (i > l)
						{
							lines.RemoveAt(i); lines.RemoveAt(l);
							lines.Insert(l, newline);
						}
						else
						{
							lines.RemoveAt(l); lines.RemoveAt(i);
							lines.Insert(i, newline);
						}
						points.RemoveAt(i);
						i = Math.Max(i - 2, -1);
						//i = -1;
						//i--;
						continue;
					}
					if (angle < 180.0)
					{
						line3d newline = new line3d(points[l], points[r]);
						bool add = true;

						for (int j = 0; j < points.Count; j++)
							if (j != l && j != r)
								if (newline.pointInside(points[j], true) && newline.A != points[j] && newline.B != points[j])
								{ add = false; break; }

						if (add)
						{
							poly3d poly = new poly3d(points[l], points[i], points[r]);
							if (poly.isNull)
							{
								if (i > l)
								{
									lines.RemoveAt(i); lines.RemoveAt(l);
									lines.Insert(l, newline);
								}
								else
								{
									lines.RemoveAt(l); lines.RemoveAt(i);
									lines.Insert(i, newline);
								}
								points.RemoveAt(i);
								i = Math.Max(i - 2, -1);
								continue;
							}
							else
							{
								for (int j = 0; j < points.Count; j++)
									if (j != l && j != i && j != r)
										if (poly.pointInside(points[j], true))
										{ add = false; break; }
								if (add)
								{
									polys.Add(poly);
									if (i > l)
									{
										lines.RemoveAt(i); lines.RemoveAt(l);
										lines.Insert(l, newline);
									}
									else
									{
										lines.RemoveAt(l); lines.RemoveAt(i);
										lines.Insert(i, newline);
									}
									points.RemoveAt(i);
									i = Math.Max(i - 2, -1);
									//i = -1;
									//i--;
								}
							}
						}
					}
				}
				//Console.WriteLine($"polys: {polys.Count}");
				return polys;
			}
			else
				return new List<poly3d>();
		}
		private List<poly3d> polysFromLoops(List<List<line3d>> loops, List<List<point3d>> loopPoints)
		{
			List<rect3d> loopRects = new List<rect3d>(loops.Count);
			for (int i = 0; i < loops.Count; i++)
			{
				double left = loopPoints[i][0].x, top = loopPoints[i][0].y, front = loopPoints[i][0].z, right = loopPoints[i][0].x, bottom = loopPoints[i][0].y, back = loopPoints[i][0].z;
				for (int j = 1; j < loopPoints[i].Count; j++)
				{
					left = Math.Min(left, loopPoints[i][j].x);
					right = Math.Max(right, loopPoints[i][j].x);
					top = Math.Min(top, loopPoints[i][j].y);
					bottom = Math.Max(bottom, loopPoints[i][j].y);
					front = Math.Min(front, loopPoints[i][j].z);
					back = Math.Max(back, loopPoints[i][j].z);
				}
				loopRects.Add(new rect3d(left, top, front, right, bottom, back));
			}
			List<double> areas = new List<double>(loops.Count);
			for (int i = 0; i < loops.Count; i++)
				areas.Add(loopRects[i].width * loopRects[i].height * Math.Max(1.0, loopRects[i].depth));
			int index = areas.IndexOf(areas.Max());
			List<line3d> pluslines = loops[index];
			List<point3d> pluspoints = loopPoints[index];
			loops.RemoveAt(index); loopPoints.RemoveAt(index);
			if (math.clockwiseDirection(pluspoints) == false)
			{
				pluspoints.Reverse(); pluslines = new List<line3d>(pluspoints.Count);
				for (int j = 0; j < pluspoints.Count; j++)
					pluslines.Add(new line3d(pluspoints[j], pluspoints[(j + 1) % pluspoints.Count]));
			}
			for (int j = 0; j < loopPoints.Count; j++)
			{
				if (math.clockwiseDirection(loopPoints[j]) == true)
				{
					loopPoints[j].Reverse(); loops[j] = new List<line3d>(loopPoints[j].Count);
					for (int k = 0; k < loopPoints[j].Count; k++)
						loops[j].Add(new line3d(loopPoints[j][k], loopPoints[j][(k + 1) % loopPoints[j].Count]));
				}
			}

			if (loops.Count == 0)
				return polysFromPoints(pluspoints);
			else
			{
				for (int i = 0; i < pluslines.Count; i++)
				{
					for (int u = 0; u < loopPoints.Count; u++)
					{
						for (int v = 0; v < loopPoints[u].Count; v++)
						{
							if (pluslines[i].pointInside(loopPoints[u][v], true))
							{
								List<point3d> path = new List<point3d>(pluspoints.Count + loopPoints[u].Count + 1);
								path.AddRange(pluspoints.GetRange(0, i + 1));
								for (int z = 0; z < loopPoints[u].Count + 1; z++)
									path.Add(loopPoints[u][(v + z) % loopPoints[u].Count]);
								if (i < pluspoints.Count - 1)
									path.AddRange(pluspoints.GetRange(i + 1, pluspoints.Count - i - 1));
								List<line3d> pathloop = new List<line3d>(path.Count);
								for (int z = 0; z < path.Count; z++)
									pathloop.Add(new line3d(path[z], path[(z + 1) % path.Count]));
								loops[u] = pathloop;
								loopPoints[u] = path;
								return polysFromLoops(loops, loopPoints);
							}
						}
					}
				}
				List<line3d> alllines = loops.SelectMany(zxc => zxc).ToList().Concat(pluslines).ToList();
				for (int i = 0; i < pluspoints.Count; i++)
				{
					for (int u = 0; u < loopPoints.Count; u++)
					{
						for (int v = 0; v < loopPoints[u].Count; v++)
						{
							line3d bridge = new line3d(pluspoints[i], loopPoints[u][v]);
							if (!bridge.overlay(alllines, true) && !bridge.crosses(alllines.Where(zxc => zxc != pluslines[i] && zxc != pluslines[(i - 1 + pluslines.Count) % pluslines.Count] && zxc != loops[u][v] && zxc != loops[u][(v - 1 + loops[u].Count) % loops[u].Count]).ToList(), false, true))
							{
								List<point3d> path = new List<point3d>(pluspoints.Count + loopPoints[u].Count + 2);
								path.AddRange(pluspoints.GetRange(0, i + 1));
								for (int z = 0; z < loopPoints[u].Count + 1; z++)
									path.Add(loopPoints[u][(v + z) % loopPoints[u].Count]);
								path.AddRange(pluspoints.GetRange(i, pluspoints.Count - i));
								List<line3d> pathloop = new List<line3d>(path.Count);
								for (int z = 0; z < path.Count; z++)
									pathloop.Add(new line3d(path[z], path[(z + 1) % path.Count]));
								loops[u] = pathloop;
								loopPoints[u] = path;
								return polysFromLoops(loops, loopPoints);
							}
						}
					}
				}
			}
			return new List<poly3d>();
		}
		public List<poly3d> simplify(List<poly3d> polys)
		{
			for (int i = 0; i < polys.Count; i++)
				if (polys[i].isNull)
				{ polys.RemoveAt(i); i--; }

			List<List<poly3d>> figures = new List<List<poly3d>>();
			List<poly3d> leftpolys = new List<poly3d>(polys);
			while (leftpolys.Count != 0)
			{
				List<poly3d> added = new List<poly3d>() { leftpolys[0] };
				leftpolys.RemoveAt(0);
				for (int i = 0; i < added.Count; i++)
				{
					for (int j = 0; j < leftpolys.Count; j++)
					{
						if (added[i].shareLine(leftpolys[j], true))
						{
							added.Add(leftpolys[j]);
							leftpolys.RemoveAt(j);
							i = -1;
							break;
						}
					}
				}
				figures.Add(added);
			}
			//Console.WriteLine($"figures: {figures.Count}");
			//for (int i = 0; i < figures.Count; i++)
			//{
			//	Console.WriteLine($"{i,2} : polys: {figures[i].Count}");
			//}
			List<List<line3d>> figurelines = new List<List<line3d>>(figures.Count);
			for (int i = 0; i < figures.Count; i++)
			{
				figurelines.Add(new List<line3d>(figures[i].Count * 3));
				for (int j = 0; j < figures[i].Count; j++)
					figurelines[i].AddRange(figures[i][j].lines());
			}

			for (int i = 0; i < figurelines.Count; i++)
			{
				for (int j = 0; j < figurelines[i].Count; j++)
				{
					List<int> indexes = new List<int>() { j };
					for (int k = j + 1; k < figurelines[i].Count; k++)
						if (figurelines[i][j] == figurelines[i][k])
							indexes.Add(k);
					if (indexes.Count > 1)
					{
						indexes.Reverse();
						for (int c = 0; c < indexes.Count; c++)
						{
							figurelines[i].RemoveAt(indexes[c]);
						}
						j--;
					}
				}
			}

			for (int i = 0; i < figurelines.Count; i++)
			{
				List<line3d> unique = new List<line3d>();
				for (int z = 0; z < figurelines[i].Count; z++)
				{
					for (int x = z + 1; x < figurelines[i].Count; x++)
						if (figurelines[i][z].overlay(figurelines[i][x], true))
						{
							List<point3d> ps = new List<point3d>()
							{
								figurelines[i][z].A,
								figurelines[i][z].B,
								figurelines[i][x].A,
								figurelines[i][x].B
							};
							for (int c = 0; c < ps.Count; c++)
							{
								for (int k = c + 1; k < ps.Count; k++)
									if (ps[c] == ps[k])
									{ ps.RemoveAt(k); k--; }
							}
							List<List<Tuple<int, int>>> indexes = new List<List<Tuple<int, int>>>(ps.Count);
							List<List<double>> lens = new List<List<double>>(ps.Count);
							for (int c = 0; c < ps.Count; c++)
							{
								indexes.Add(new List<Tuple<int, int>>(ps.Count - 1));
								lens.Add(new List<double>(ps.Count - 1));
								for (int k = 0; k < ps.Count; k++)
									if (c != k)
									{
										indexes[c].Add(new Tuple<int, int>(c, k));
										lens[c].Add((ps[c] - ps[k]).length);
									}
							}
							List<line3d> lines = new List<line3d>();
							for (int c = 0; c < ps.Count; c++)
							{
								int id = lens[c].IndexOf(lens[c].Min());
								lines.Add(new line3d(ps[indexes[c][id].Item1], ps[indexes[c][id].Item2]));
							}
							for (int c = 0; c < lines.Count; c++)
							{
								for (int k = c + 1; k < lines.Count; k++)
									if (lines[c] == lines[k])
									{ lines.RemoveAt(k); k--; }
							}
							for (int c = 0; c < lines.Count; c++)
							{
								if (figurelines[i][z].overlay(lines[c], true) && figurelines[i][x].overlay(lines[c], true))
								{ lines.RemoveAt(c); c--; }
							}
							figurelines[i].RemoveAt(x);
							figurelines[i].RemoveAt(z);
							figurelines[i].InsertRange(z, lines);
							z = -1;
							break;
						}
				}
			}

			for (int i = 0; i < figurelines.Count; i++)
			{
				for (int j = 0; j < figurelines[i].Count; j++)
				{
					List<int> indexes = new List<int>() { j };
					for (int k = j + 1; k < figurelines[i].Count; k++)
						if (figurelines[i][j] == figurelines[i][k])
							indexes.Add(k);
					if (indexes.Count > 1)
					{
						indexes.Reverse();
						for (int c = 0; c < indexes.Count; c++)
						{
							figurelines[i].RemoveAt(indexes[c]);
						}
						j--;
					}
				}
			}

			for (int i = 0; i < figurelines.Count; i++)
			{
				for (int j = 0; j < figurelines[i].Count; j++)
				{
					for (int k = j + 1; k < figurelines[i].Count; k++)
					{
						if (figurelines[i][j].continues(figurelines[i][k], true))
						{
							List<point3d> ps = new List<point3d>()
							{
								figurelines[i][j].A,
								figurelines[i][j].B,
								figurelines[i][k].A,
								figurelines[i][k].B
							};
							for (int u = 0; u < ps.Count; u++)
							{
								for (int v = u + 1; v < ps.Count; v++)
									if (ps[u] == ps[v])
									{ ps.RemoveAt(v); v--; }
							}
							List<double> lens = new List<double>();
							List<Tuple<int, int>> ids = new List<Tuple<int, int>>();
							for (int u = 0; u < ps.Count; u++)
								for (int v = 0; v < ps.Count; v++)
									if (v != u)
									{
										lens.Add((ps[u] - ps[v]).length);
										ids.Add(new Tuple<int, int>(u, v));
									}
							int id = lens.IndexOf(lens.Max());
							figurelines[i][j] = new line3d(ps[ids[id].Item1], ps[ids[id].Item2]);
							figurelines[i].RemoveAt(k);
							j--;
							break;
						}
					}
				}
			}
			//Console.WriteLine($"figurelines: {figurelines.Count}");
			//for (int i = 0; i < figurelines.Count; i++)
			//{
			//	Console.WriteLine($"{i,2} : lines: {figurelines[i].Count}");
			//}
			List<poly3d> ret = new List<poly3d>();
			for (int i = 0; i < figures.Count; i++)
			{
				//int errors = 0;
				List<List<line3d>> loops = new List<List<line3d>>();
				List<List<point3d>> loopPoints = new List<List<point3d>>();
				List<line3d> leftlines = new List<line3d>(figurelines[i]);
				while (leftlines.Count != 0)
				{
					List<line3d> newlines = new List<line3d>() { leftlines[0] };
					List<point3d> breakPoints = new List<point3d>();
					List<int> breakIds = new List<int>();
					leftlines.RemoveAt(0);
					for (int j = 0; j < newlines.Count; j++)
					{
						List<int> ids = new List<int>();
						for (int k = 0; k < leftlines.Count; k++)
							if ((leftlines[k].A == newlines[^1].B || leftlines[k].B == newlines[^1].B))
								ids.Add(k);

						if (ids.Count == 0)
						{
							Console.WriteLine("Could not find any fitting line!");
							newlines = new List<line3d>();
							loops = new List<List<line3d>>();
							loopPoints = new List<List<point3d>>();
							leftlines = new List<line3d>();
							ret.AddRange(simplifyRandom(figures[i]));
							/*
							Console.WriteLine($"Could not find any fitting line! errors: {errors}");
							newlines = new List<line3d>();
							loops = new List<List<line3d>>();
							loopPoints = new List<List<point3d>>();
							if (errors < 2)
							{
								errors++;
								leftlines = new List<line3d>(figurelines[i]).OrderBy(zxc => math.rnd.Next()).ToList();
							}
							else
							{
								leftlines = new List<line3d>();
								ret.AddRange(simplifyRandom(figures[i]));
							}
							*/
							break;
						}
						else if (ids.Count == 1)
						{
							int id = ids[0];

							newlines.Add(leftlines[id]);
							leftlines.RemoveAt(id);

							if (newlines[^1].A != newlines[^2].B)
								newlines[^1] = newlines[^1].reversed();

							if (newlines[^1].B.inlist(breakPoints))
							{
								int start = breakPoints.IndexOf(newlines[^1].B);
								start = breakIds[start];
								leftlines.AddRange(newlines.GetRange(0, start));
								newlines = newlines.GetRange(start, newlines.Count - start);
								break;
							}
							else if (newlines[^1].B == newlines[0].A)
							{
								// loop ended successfully
								break;
							}
							else
							{
								// continue searching
								continue;
							}
						}
						else
						{
							bool loopEnd = false;
							for (int k = 0; k < ids.Count; k++)
							{
								int id = ids[k];
								line3d test = leftlines[id];
								if (test.A != newlines[^1].B)
									test = test.reversed();
								if (test.B.inlist(breakPoints))
								{
									newlines.Add(test);
									leftlines.RemoveAt(id);
									int start = breakPoints.IndexOf(newlines[^1].B);
									start = breakIds[start];
									leftlines.AddRange(newlines.GetRange(0, start));
									newlines = newlines.GetRange(start, newlines.Count - start);
									loopEnd = true;
									break;
								}
								else if (test.B == newlines[0].A)
								{
									newlines.Add(test);
									leftlines.RemoveAt(id);
									loopEnd = true;
									break;
								}
							}
							if (!loopEnd)
							{
								breakPoints.Add(newlines[^1].B);
								breakIds.Add(newlines.Count);

								int id = ids[math.rnd.Next(ids.Count)];
								newlines.Add(leftlines[id]);
								leftlines.RemoveAt(id);

								if (newlines[^1].A != newlines[^2].B)
									newlines[^1] = newlines[^1].reversed();
							}
							else
								break;
						}
					}
					if (newlines.Count > 2)
					{
						//Console.WriteLine($"loop FOUND");
						loops.Add(newlines);
						loopPoints.Add(newlines.Select(zxc => zxc.A).ToList());
					}
				}
				if (loops.Count > 1)
					ret.AddRange(polysFromLoops(loops, loopPoints));
				else if (loops.Count == 1)
					ret.AddRange(polysFromPoints(loopPoints[0]));
				else
					continue;
			}
			return ret;
		}
		public List<poly3d> simplifyRandom(List<poly3d> polys)
		{
			for (int i = 0; i < polys.Count; i++)
				if (polys[i].isNull)
				{ polys.RemoveAt(i); i--; }
			List<List<line3d>> all = new List<List<line3d>>();
			for (int i = 0; i < polys.Count; i++)
				all.Add(polys[i].lines());
			Start:
			for (int i = 0; i < all.Count; i++)
				for (int u = i + 1; u < all.Count; u++)
					if (polys[i].rect3d.intersect(polys[u].rect3d))
						for (int j = 0; j < 3; j++)
							for (int v = 0; v < 3; v++)
								if (all[i][j] == all[u][v])
									for (int k = 0; k < 3; k++)
										if (k != j)
											for (int w = 0; w < 3; w++)
												if (w != v)
													//if (all[i][k].continues(all[u][w], true))
													//if (all[i][k].shareEdgePoint(all[u][w], true) && all[i][k].sameStraight(all[u][w], true) && all[i][k] != all[u][w])
													//if (all[i][k].sameStraight(all[u][w], true) && all[i][k] != all[u][w])
													if (all[i][k].sameStraight(all[u][w], true))
														if (combine(i, j, k, u, v, w))
															goto Start;
			return polys.Where(poly => !poly.isNull).ToList();
			bool combine(int i, int j, int k, int u, int v, int w)
			{
				List<int> ids1 = new List<int>() { 0, 1, 2 };
				ids1.Remove(j); ids1.Remove(k);
				List<int> ids2 = new List<int>() { 0, 1, 2 };
				ids2.Remove(v); ids2.Remove(w);
				List<line3d> lines = new List<line3d>() { all[i][ids1[0]], all[u][ids2[0]] };
				List<point3d> ps = new List<point3d>()
				{
					all[i][k].A,
					all[i][k].B,
					all[u][w].A,
					all[u][w].B
				};
				bool done = false;
				for (int z = 0; z < ps.Count && !done; z++)
					for (int x = z + 1; x < ps.Count; x++)
						if (ps[z] == ps[x])
						{ ps.RemoveAt(x); ps.RemoveAt(z); done = true; break; }

				if (!lines[0].A.inlist(ps))
					ps.Add(lines[0].A);
				else if (!lines[0].B.inlist(ps))
					ps.Add(lines[0].B);
				else if (!lines[1].A.inlist(ps))
					ps.Add(lines[1].A);
				else if (!lines[1].B.inlist(ps))
					ps.Add(lines[1].B);

				if (ps.Count == 3)
				{
					poly3d modified = new poly3d(ps[0], ps[1], ps[2]);
					if (!modified.isNull)
					{
						polys[i] = modified;
						all[i] = modified.lines();
						polys.RemoveAt(u);
						all.RemoveAt(u);
						return true;
					}
				}
				Console.WriteLine($"SimplifyRandom method combine caught strange result! points: {ps.Count}");
				return false;
			}
		}
		public poly3d cloneNoZ()
		{
			if (this.isNull)
				return new poly3d();
			return new poly3d(this.A.cloneNoZ(), this.B.cloneNoZ(), this.C.cloneNoZ());
		}
		public poly3d clone()
		{
			if (this.isNull)
				return new poly3d();
			return new poly3d(this.A, this.B, this.C);
		}
		public List<line3d> lines()
		{
			if (this.isNull || this.alllines.Count == 0)
				return new List<line3d>();
			return new List<line3d>(this.alllines);
		}
		public static bool operator ==(poly3d A, poly3d B)
		{
			if (A.isNull && B.isNull)
				return true;
			if (!A.isNull && !B.isNull)
				if (anyof(A.A, B) && anyof(A.B, B) && anyof(A.C, B))
					return true;
			return false;
			bool anyof(point3d p, poly3d to)
			{
				return p == to.A || p == to.B || p == to.C;
			}
		}
		public static bool operator !=(poly3d A, poly3d B)
		{
			return !(A == B);
		}
		public override bool Equals(object? obj)
		{
			if (obj == null)
				return false;
			if (!(obj is poly3d))
				return false;
			return this == (poly3d)obj;
		}
		public override int GetHashCode()
		{
			return Tuple.Create(A, B, C, isNull).GetHashCode();
		}
		public string str()
		{
			if (this.isNull)
				return "null";
			return $"A: {A.str()}\nB: {B.str()}\nC: {C.str()}";
		}
		private void toNull()
		{
			this.A = new point3d();
			this.B = new point3d();
			this.C = new point3d();
			this.eq = Array.Empty<double>();
			this.norm = new point3d();
			this.area = new double();
			this.areaz = new double();
			this.centerpoint = new point3d();
			this.rect = new Rectangle();
			this.rect3d = new rect3d();
			this.allpoints = new List<Point>();
			this.alllines = new List<line3d>();
			this.isNull = true;
		}
	}
	/*
	public struct plane3d
	{
		List<poly3d> allpolys { get; set; }
		public double[] eq { get; set; }
		public bool isNull { get; set; }
		public plane3d()
		{
			toNull();
			return;
		}
		public plane3d(poly3d poly)
		{
			if (poly.isNull)
			{
				toNull();
				return;
			}
			this.allpolys = new List<poly3d>() { poly };
			this.eq = poly.eq;
			this.isNull = false;
		}
		public plane3d(List<poly3d> polys)
		{
			for (int i = 0; i < polys.Count; i++)
				if (polys[i].isNull)
				{ polys.RemoveAt(i); i--; }

			List<List<poly3d>> planes = new List<List<poly3d>>();
			List<List<double>> eqs = new List<List<double>>();
			List<int> reserved = new List<int>();
			for (int i = 0; i < polys.Count; i++)
			{
				if (!reserved.Contains(i))
				{
					reserved.Add(i);
					eqs.Add(polys[i].eq.ToList());
					planes.Add(allpolys[i]);
					for (int j = i + 1; j < polys.Count; j++)
					{
						if (!reserved.Contains(j))
							if (polys[i].sameSurface(polys[j]))
							{
								planes[^1].AddRange(allpolys[j]);
								reserved.Add(j);
							}
					}
				}
			}
		}
		private void toNull()
		{
			this.allpolys = new List<poly3d>();
			this.eq = Array.Empty<double>();
			this.isNull = true;
		}
	}
	*/
	public struct shape3d
	{
		private List<poly3d> pluspolys { get; set; }
		private List<poly3d> minuspolys { get; set; }
		private bool simplified { get; set; }
		private bool calculated { get; set; }
		private Rectangle bounds { get; set; }
		private List<Point> allpoints { get; set; }
		public bool isNull { get; set; }
		public shape3d() { toNull(); }
		public shape3d(poly3d poly)
		{
			if (poly.isNull)
			{
				toNull();
				return;
			}
			this.pluspolys = new List<poly3d>() { poly };
			this.minuspolys = new List<poly3d>();
			this.simplified = true;
			this.calculated = true;
			setBounds();
			this.allpoints = new List<Point>();
			this.isNull = false;
		}
		public shape3d(List<poly3d> pluspolys)
		{
			if (pluspolys.Count == 0)
			{
				toNull();
				return;
			}
			this.pluspolys = pluspolys;
			this.minuspolys = new List<poly3d>();
			this.simplified = false;
			this.calculated = true;
			setBounds();
			this.allpoints = new List<Point>();
			this.isNull = false;
		}
		private shape3d(List<poly3d> pluspolys, bool simplified)
		{
			if (pluspolys.Count == 0)
			{
				toNull();
				return;
			}
			this.pluspolys = pluspolys;
			this.minuspolys = new List<poly3d>();
			this.simplified = simplified;
			this.calculated = true;
			setBounds();
			this.allpoints = new List<Point>();
			this.isNull = false;
		}
		public shape3d(List<poly3d> pluspolys, List<poly3d> minuspolys)
		{
			if (pluspolys.Count == 0)
			{
				toNull();
				return;
			}
			if (minuspolys.Count == 0)
			{
				this = new shape3d(pluspolys);
				return;
			}
			this.pluspolys = pluspolys;
			this.minuspolys = minuspolys;
			this.simplified = false;
			this.calculated = false;
			this.allpoints = new List<Point>();
			this.isNull = false;
		}
		public shape3d(List<point3d> points)
		{
			//List<poly3d> polys = new poly3d().simplifyRandom(new poly3d().polysFromPoints(points));
			List<poly3d> polys = new poly3d().polysFromPoints(points);
			if (polys.Count == 0)
			{
				toNull();
				return;
			}
			this.pluspolys = polys;
			this.minuspolys = new List<poly3d>();
			this.simplified = true;
			this.calculated = true;
			setBounds();
			this.allpoints = new List<Point>();
			this.isNull = false;
		}
		public List<poly3d> polys()
		{
			if (this.isNull)
				return new List<poly3d>();

			calculate();
			simplify();

			return new List<poly3d>(this.pluspolys);
		}
		public void calculate()
		{
			if (this.calculated || this.isNull)
				return;

			//List<poly3d> plus = new List<poly3d>(this.pluspolys);
			//List<poly3d> minus = new List<poly3d>(this.minuspolys);
			List<poly3d> plus = new poly3d().summarize(this.pluspolys);
			List<poly3d> minus = new poly3d().summarize(this.minuspolys);

			List<poly3d> result = new List<poly3d>();
			for (int i = 0; i < plus.Count; i++)
				result.AddRange(plus[i].cut(minus.ToList()));

			this.pluspolys = result;
			this.minuspolys = new List<poly3d>();
			this.simplified = false;
			this.calculated = true;
			setBounds();
		}
		public void simplify()
		{
			if (!this.calculated)
				calculate();
			if (this.simplified || this.isNull)
				return;

			this.pluspolys = new poly3d().summarize(this.pluspolys);
			this.simplified = true;
		}
		public shape3d result()
		{
			calculate();
			simplify();
			return this;
		}
		public List<Point> allPoints()
		{
			if (this.isNull) return new List<Point>();
			calculate();
			simplify();
			if (this.allpoints.Count != 0) return new List<Point>(this.allpoints);
			List<Point> points = this.pluspolys.SelectMany(zxc => zxc.allPoints()).ToList();
			this.allpoints = points;
			return new List<Point>(this.allpoints);
		}
		public Point randpoint()
		{
			if (allPoints().Count == 0)
				return new Point();
			return allpoints[math.rnd.Next(allpoints.Count)];
		}
		private void setBounds()
		{
			if (this.isNull || this.pluspolys.Count == 0)
			{ this.bounds = new Rectangle(); return; }
			int left = this.pluspolys[0].rect.Left;
			int right = this.pluspolys[0].rect.Right;
			int top = this.pluspolys[0].rect.Top;
			int bottom = this.pluspolys[0].rect.Bottom;
			for (int i = 1; i < this.pluspolys.Count; i++)
			{
				left = Math.Min(left, this.pluspolys[i].rect.Left);
				right = Math.Max(right, this.pluspolys[i].rect.Right);
				top = Math.Min(top, this.pluspolys[i].rect.Top);
				bottom = Math.Max(bottom, this.pluspolys[i].rect.Bottom);
			}
			if (left < right && top < bottom)
				this.bounds = new Rectangle(left, top, right - left, bottom - top);
			else
				this.bounds = new Rectangle();
		}
		public Rectangle rect()
		{
			calculate();
			simplify();
			return this.bounds;
		}
		public shape3d add(List<shape3d> shapes) { return add(shapes, calculationSettings.autoCalculate); }
		public shape3d add(List<shape3d> shapes, bool calc)
		{
			shapes.ForEach(shape => shape.result());
			if (calc)
				return new shape3d(this.polys().Concat(shapes.SelectMany(shape => shape.polys())).ToList());
			else
				return new shape3d(this.pluspolys.Concat(shapes.SelectMany(shape => shape.polys())).ToList(), this.minuspolys.ToList());
		}
		public shape3d add(shape3d shape) { return add(shape, calculationSettings.autoCalculate); }
		public shape3d add(shape3d shape, bool calc)
		{
			shape = shape.result();
			if (calc)
				return new shape3d(this.polys().Concat(shape.polys()).ToList());
			else
				return new shape3d(this.pluspolys.Concat(shape.polys()).ToList(), this.minuspolys.ToList());
		}
		public shape3d add(List<poly3d> polys) { return add(polys, calculationSettings.autoCalculate); }
		public shape3d add(List<poly3d> polys, bool calc)
		{
			if (calc)
				return new shape3d(this.polys().Concat(polys).ToList());
			else
				return new shape3d(this.pluspolys.Concat(polys).ToList(), this.minuspolys.ToList());
		}
		public shape3d add(poly3d poly) { return add(poly, calculationSettings.autoCalculate); }
		public shape3d add(poly3d poly, bool calc)
		{
			if (calc)
				return new shape3d(this.polys().Append(poly).ToList());
			else
				return new shape3d(this.pluspolys.Append(poly).ToList(), this.minuspolys.ToList());
		}
		public shape3d cut(List<shape3d> shapes) { return cut(shapes, calculationSettings.autoCalculate); }
		public shape3d cut(List<shape3d> shapes, bool calc)
		{
			shapes.ForEach(shape => shape.result());
			if (calc)
				return new shape3d(this.polys(), shapes.SelectMany(shape => shape.polys()).ToList());
			else
				return new shape3d(this.pluspolys.ToList(), this.minuspolys.Concat(shapes.SelectMany(shape => shape.polys())).ToList());
		}
		public shape3d cut(shape3d shape) { return cut(shape, calculationSettings.autoCalculate); }
		public shape3d cut(shape3d shape, bool calc)
		{
			shape = shape.result();
			if (calc)
				return new shape3d(this.polys(), shape.polys());
			else
				return new shape3d(this.pluspolys.ToList(), this.minuspolys.Concat(shape.polys()).ToList());
		}
		public shape3d cut(List<poly3d> polys) { return cut(polys, calculationSettings.autoCalculate); }
		public shape3d cut(List<poly3d> polys, bool calc)
		{
			if (calc)
				return new shape3d(this.polys(), polys);
			else
				return new shape3d(this.pluspolys.ToList(), this.minuspolys.Concat(polys).ToList());
		}
		public shape3d cut(poly3d poly) { return cut(poly, calculationSettings.autoCalculate); }
		public shape3d cut(poly3d poly, bool calc)
		{
			if (calc)
				return new shape3d(this.polys(), new List<poly3d>() { poly });
			else
				return new shape3d(this.pluspolys.ToList(), this.minuspolys.Append(poly).ToList());
		}
		private void toNull()
		{
			this.pluspolys = new List<poly3d>();
			this.minuspolys = new List<poly3d>();
			this.simplified = true;
			this.calculated = true;
			this.bounds = new Rectangle();
			this.allpoints = new List<Point>();
			this.isNull = true;
		}
	}
}