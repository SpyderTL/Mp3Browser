using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mp3Browser.TreeNodes
{
	public class Mp3Header : DataNode
	{
		public IReadable Source;
		public int Offset;

		public Mp3Header() : base(false)
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

				var header = reader.ReadBytes(4);

				var version = (header[1] >> 3) & 0x03;
				var layer = (header[1] >> 1) & 0x03;
				var noCrc = header[1] & 0x01;

				var bitRate = (header[2] >> 4) & 0x0f;
				var sampleRate = (header[2] >> 2) & 0x03;
				var padding = (header[2] >> 1) & 0x01;
				var privateFlag = header[2] & 0x01;

				var channelMode = (header[3] >> 6) & 0x02;
				var msStereo = (header[3] >> 4) & 0x01;
				var intensityStereo = (header[3] >> 3) & 0x01;
				var copyright = (header[3] >> 2) & 0x01;
				var original = (header[3] >> 1) & 0x01;
				var emphasis = (header[3] >> 0) & 0x01;

				var bitRates = new int[] { 0, 8000, 16000, 24000, 32000, 40000, 48000, 56000, 64000, 80000, 96000, 112000, 128000, 144000, 160000 };
				var sampleRates = new int[] { 22050, 24000, 16000 };

				if (bitRate < bitRates.Length &&
					sampleRate < sampleRates.Length &&
					bitRates[bitRate] != 0)
				{
					bitRate = bitRates[bitRate];
					sampleRate = sampleRates[sampleRate];
				}

				return new
				{
					Version = version,
					Layer = layer,
					NoCRC = noCrc,
					BitRate = bitRate,
					SampleRate = sampleRate,
					Padding = padding,
					Private = privateFlag,
					ChannelMode = channelMode,
					MSStereo = msStereo,
					IntensityStereo = intensityStereo,
					Copyright = copyright,
					Original = original,
					Emphasis = emphasis,
					Position = Offset
				};
			}
		}
	}
}