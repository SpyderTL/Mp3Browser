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
					var position = reader.BaseStream.Position;

					var header = reader.ReadBytes(4);

					if (header[0] == 'I' &&
						header[1] == 'D' &
						header[2] == '3')
					{
						reader.BaseStream.Seek(-4, SeekOrigin.Current);

						var tag = reader.ReadBytes(3);
						var version = reader.ReadBytes(2);
						var flags = reader.ReadByte();
						var size = reader.ReadBytes(4);

						Nodes.Add(new Id3Tag { Text = "ID3 Tag", Offset = (int)position, Source = this });

						var length = size[0] << 21 | size[1] << 14 | size[2] << 7 | size[3];

						var tags = reader.ReadBytes(length);
					}
					else if (header[0] == 0xff &&
						(header[1] & 0xf0) == 0xf0)
					{
						var version = (header[1] >> 3) & 0x03;
						var layer = (header[1] >> 1) & 0x03;
						var noCrc = header[1] & 0x01;

						var bitRate = (header[2] >> 4) & 0x0f;
						var sampleRate = (header[2] >> 2) & 0x03;
						var padding = (header[2] >> 1) & 0x01;
						var privateFlag = header[2] & 0x01;

						var channelMode = (header[3] >> 6) & 0x02;
						var msStereo = (header[3] >> 5) & 0x01;
						var intensityStereo = (header[3] >> 4) & 0x01;
						var copyright = (header[3] >> 3) & 0x01;
						var original = (header[3] >> 2) & 0x01;
						var emphasis = (header[3] >> 0) & 0x02;

						var bitRates = new int[] { 0, 8000, 16000, 24000, 32000, 40000, 48000, 56000, 64000, 80000, 96000, 112000, 128000, 144000, 160000 };
						//var bitRates = new int[] { 0, 32000, 40000, 48000, 56000, 64000, 80000, 96000, 112000, 128000, 160000, 192000, 224000, 256000, 320000 };
						//var sampleRates = new int[] { 22050, 24000, 16000 };
						var sampleRates = new int[] { 44100, 48000, 32000 };

						if (bitRate < bitRates.Length &&
							sampleRate < sampleRates.Length &&
							bitRates[bitRate] != 0)
						{
							bitRate = bitRates[bitRate];
							sampleRate = sampleRates[sampleRate];

							var dataLength = ((144 * bitRate) / sampleRate) + padding - 4;

							Nodes.Add(new Mp3Header { Text = "MP3 Header", Offset = (int)position, Source = this });

							//if (noCrc == 0)
							//	reader.ReadBytes(2);

							var data = reader.ReadBytes((int)dataLength);
						}
						else
							reader.BaseStream.Seek(-3, SeekOrigin.Current);
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