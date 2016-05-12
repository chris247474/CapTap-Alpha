using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;

namespace Capp2
{
	public interface IPics
	{
		IEnumerable<Tesseract.Result> imageResult{ get; set;}
		Task<IEnumerable<Tesseract.Result>> loadFromPicDivideBy (Tesseract.PageIteratorLevel pageLevel, Stream s);
		Stream SaturationTo0(Stream image);
		void SaveImageToDisk(Stream s, string filename);
		Stream PreprocessImage(Stream image, double GaussianSizeX, double GaussianSizeY);
	}
}

