using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mp3Browser.TreeNodes
{
	public class Id3Tag : DataNode
	{
		public IReadable Source;
		public int Offset;

		public Id3Tag() : base(false)
		{
		}

		public override void Reload()
		{
		}

		public override object GetProperties()
		{
			using (var reader = new BinaryReader(Source.GetStream()))
			{
				reader.BaseStream.Position = Offset;

				var tag = reader.ReadBytes(3);
				var version = reader.ReadBytes(2);
				var flags = reader.ReadByte();
				var size = reader.ReadBytes(4);

				var length = size[0] << 21 | size[1] << 14 | size[2] << 7 | size[3];

				var tags = reader.ReadBytes(length);

				return new
				{
					Position = Offset,
					Tag = tag,
					Version = version,
					Flags = flags,
					Size = length
				};
			}
		}
	}
}

