using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mp3Browser.TreeNodes
{
	public class Mp3File : DataNode, IReadable
	{
		public string Path;

		public Mp3File() : base(true)
		{
		}

		public override void Reload()
		{
			Nodes.Clear();

			using (var reader = new BinaryReader(GetStream()))
			{
				while (reader.BaseStream.Position < reader.BaseStream.Length - 32)
				{
					var header = BitConverter.ToUInt32(reader.ReadBytes(4).Reverse().ToArray(), 0);

					var sync = (header >> 16);

					if (sync == 0xfff2)
					{
						var version = (header >> 19) & 0x01;
						var layer = (header >> 17) & 0x03;
						var noCrc = (header >> 15) & 0x01;
						var bitRate = (header >> 12) & 0x0f;
						var sampleRate = (header >> 10) & 0x03;
						var padding = (header >> 8) & 0x01;

						var bitRates = new int[] { 0, 0, 0, 0, 0, 0, 0, 96000, 112000, 128000, 192000, 256000, 320000 };
						var sampleRates = new int[] { 44100, 48000, 32000 };
					}
					else
						reader.BaseStream.Seek(-3, SeekOrigin.Current);
				}
			}
		}


		public override object GetProperties()
		{
			return new
			{
				Path,
				new FileInfo(Path).Length
			};
		}

		public Stream GetStream()
		{
			return File.OpenRead(Path);
		}
	}
}