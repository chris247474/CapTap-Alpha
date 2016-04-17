using System;
using Tesseract.Droid;
using Xamarin.Forms;
using System.Collections.Generic;
using Capp2.Droid;
using System.Threading.Tasks;
using System.IO;
using Xamarin.Forms.Platform.Android;
using Android.Graphics;
using Android.Content;
using OpenCV.Core;
using OpenCV.ImgProc;
using OpenCV.Android;
using System.Linq;
using OpenCV.Utils;
using Android.Runtime;
using Java.Util;
using Java.IO;

[assembly: Dependency (typeof (Pics))]

namespace Capp2.Droid
{
	public class Pics:IPics 
	{
		public IEnumerable<Tesseract.Result> imageResult{ get; set;}
		public Pics ()
		{
		}

		public Stream PreprocessImage(Stream image, double GaussianSizeX, double GaussianSizeY){
			try{
				int type = CvType.Cv8uc1;
					Bitmap bmpImage = BytesToBitmap (StreamToBytes (image));
					Bitmap bmpTemp = BytesToBitmap (StreamToBytes (image));//
					Mat img = new Mat(bmpImage.Height, bmpImage.Width, type);
				OpenCV.Android.Utils.BitmapToMat (bmpImage, img);
					Mat sharp = createMat (img, type), 
				finalImage = createMat (img, type), grey = createMat (img, type), tmpGrey = createMat (img, type), tempFinal = createMat (img, type);

				checkValue (sharp, "sharp");
				checkValue (finalImage, "finalImage");
				checkValue (img, "img");

				Imgproc.CvtColor(img,grey,Imgproc.ColorBgr2gray);
				/*grey.CopyTo (tmpGrey);

					//testing perspective transform, auto crop
					tempFinal = CropText(tmpGrey, 50, img, type);
					OpenCV.Android.Utils.MatToBitmap (tempFinal, bmpTemp);
					SaveImageToDisk(ToStream(BitmapToBytes(bmpTemp)), "cropped, perspective warp.jpg");*/

				sharp = UnSharpv1(grey, img, type, 101, 101, 1);
				//sharp = UnSharpv1(sharp, img, type, Values.GAUSSIANSIZEX, Values.GAUSSIANSIZEY);
				//sharp = UnSharpFromDocs(grey, img, type, 3, 3, 1, 5, 0);
				//sharp = UnSharpTest(grey, img, type, 3, 3, 1, 5, 0);

				//black and white
				Imgproc.AdaptiveThreshold(sharp, finalImage, 255, Imgproc.AdaptiveThreshGaussianC, Imgproc.ThreshBinary, Values.ADAPTIVETHRESHBLOCKSIZE, Values.ADAPTIVETHRESHPARAM);

				OpenCV.Android.Utils.MatToBitmap (finalImage, bmpImage);
				return ToStream(BitmapToBytes(bmpImage));
			}catch(Exception e){
				System.Console.WriteLine ("[Droid.Pics] ERROR SHARPENING IMAGE: "+e.Message);
			}
			return null;
		}

		/*public Stream PreprocessImage(Stream image, double GaussianSizeX, double GaussianSizeY){
			try{
				int type = CvType.Cv8uc1;
				Bitmap bmpImage = BytesToBitmap (StreamToBytes (image));
				Bitmap bmpTemp = BytesToBitmap (StreamToBytes (image));//
				Mat img = new Mat(bmpImage.Height, bmpImage.Width, type);
				OpenCV.Android.Utils.BitmapToMat (bmpImage, img);
				Mat sharp = createMat (img, type), 
				finalImage = createMat (img, type), grey = createMat (img, type), tmpGrey = createMat (img, type), tempFinal = createMat (img, type);

				checkValue (sharp, "sharp");
				checkValue (finalImage, "finalImage");
				checkValue (img, "img");


				Imgproc.CvtColor(img,grey,Imgproc.ColorBgr2gray);
				grey.CopyTo (tmpGrey);

				sharp = UnSharpv1(grey, img, type, 101, 101, 1);

				//black and white
				Imgproc.AdaptiveThreshold(sharp, finalImage, 255, Imgproc.AdaptiveThreshGaussianC, Imgproc.ThreshBinary, Values.ADAPTIVETHRESHBLOCKSIZE, Values.ADAPTIVETHRESHPARAM);

				OpenCV.Android.Utils.MatToBitmap (finalImage, bmpImage);
				return ToStream(BitmapToBytes(bmpImage));
			}catch(Exception e){
				System.Console.WriteLine ("[Droid.Pics] ERROR SHARPENING IMAGE: "+e.Message);
			}
			return null;
		}*/

		/*public Mat JavaCropText(Mat grey, int lowThresh, Mat img, int type){
			// from https://github.com/vipul-sharma20/document-scanner/blob/master/scanner.py
			Mat edges = createMat (img, type), blurred = createMat (img, type), heirarchy = createMat(img, type), orig = createMat(img, type);
			ArrayList<MatOfPoint> javaContourList = new ArrayList<MatOfPoint> ();
			List<MatOfPoint> contourList = new List<MatOfPoint>(),  targetList = new List<MatOfPoint>();
			List<MatOfPoint2f> contourList2f = new List<MatOfPoint2f>();
			MatOfPoint2f approx = new MatOfPoint2f(), target = new MatOfPoint2f();

			//edges.SetTo (new Scalar (0));

			grey.CopyTo (orig);
			System.Console.WriteLine ("DONE COPYING GREY TO ORIG");

			Imgproc.GaussianBlur(grey, blurred, new OpenCV.Core.Size(3, 3), 0);
			System.Console.WriteLine ("GAUSSIAN DONE");
			Imgproc.Canny (blurred, edges, 0, 50); //Imgproc.Canny (blurred, edges, lowThresh, lowThresh * 3);
			System.Console.WriteLine ("CANNY DONE");
			//grey.CopyTo (edges, blurred);
			//System.Console.WriteLine ("COPIED BLURRED TO EDGES");

			//return edges;

			Imgproc.FindContours (edges, javaContourList, heirarchy, Imgproc.RetrList, Imgproc.ChainApproxNone);//Imgproc.RetrExternal ?
			System.Console.WriteLine ("contourlist  count: "+javaContourList.Count.ToString ());
			System.Console.WriteLine ("FINDCONTOURS DONE");
			for(int i = 0; i < javaContourList.Count; i++){
				MatOfPoint2f myPt = new MatOfPoint2f();
				javaContourList.ElementAt(i).ConvertTo(myPt, CvType.Cv32fc2);//change? //type
				contourList2f.Add(myPt);
			}
			System.Console.WriteLine ("CONVERT TO MATOIPOINT2f DONE");
			contourList2f.OrderByDescending (x => Imgproc.ContourArea (x)).ToList ();

			//get approximate contour
			//might need to replace with a better find-largest-contour implenetation
			System.Console.WriteLine ("contourlist2f  count: "+contourList2f.Count.ToString ());
			foreach(MatOfPoint2f c in contourList2f){
				var p = Imgproc.ArcLength(c, true);
				Imgproc.ApproxPolyDP (c, approx, 0.02 * p, true);
				if (approx == null) {
					System.Console.WriteLine ("APPROX is null");
				}

				System.Console.WriteLine ("[APPROX ] "+approx.ToList ().Count.ToString ());

				if (approx.ToList ().Count == 4) {
					System.Console.WriteLine ("APPROX HAS 4 ELEMENTS");
					target = approx;
				}
			}
			System.Console.WriteLine ("GET APPROX CONTOUR DONE");
			//mapping target points to 800x800 quadrilateral?
			// from http://stackoverflow.com/questions/21084098/how-to-find-the-corners-of-a-rect-object-in-opencv
			//Imgproc.cvtColor(imgSource, imgSource, Imgproc.COLOR_BayerBG2RGB); //just in case (above link converts it back to color but ill try keeping it grey since i have further uses for the image)
			double[] temp_double = approx.Get(0,0);       
			System.Console.WriteLine ("temp_double size: "+approx.Get(0,0).ElementAt ((0)));
			OpenCV.Core.Point p1 = new OpenCV.Core.Point(temp_double[0], temp_double[1]);
			System.Console.WriteLine ("POINT (0,0) DONE");
			temp_double = approx.Get(1,0);    
			OpenCV.Core.Point p2 = new OpenCV.Core.Point(temp_double[0], temp_double[1]);
			System.Console.WriteLine ("POINT (1,0) DONE");
			temp_double = approx.Get(2,0);       
			OpenCV.Core.Point p3 = new OpenCV.Core.Point(temp_double[0], temp_double[1]);
			System.Console.WriteLine ("POINT (2,0) DONE");
			temp_double = approx.Get(3,0);       
			OpenCV.Core.Point p4 = new OpenCV.Core.Point(temp_double[0], temp_double[1]);
			System.Console.WriteLine ("POINT (3,0) DONE");
			List<OpenCV.Core.Point> source = new List<OpenCV.Core.Point>();
			source.Add(p1);
			source.Add(p2);
			source.Add(p3);
			source.Add(p4);
			System.Console.WriteLine ("SOURCELIST DONE");
			Mat startM = Converters.Vector_Point2f_to_Mat(source);//need to init startM?
			Mat result=warp(orig,startM, type);//either orig or grey //need to init result?
			System.Console.WriteLine ("WARP() DONE");
			//////////////////////////////////////////////////////////////////////////////////////////////////////
			System.Console.WriteLine ("TARGET POINTS MAPPED");

			MatOfPoint targetMatofPoint = new MatOfPoint();
			target.ConvertTo (targetMatofPoint, type);
			targetList.Add (targetMatofPoint);
			Imgproc.DrawContours (grey, targetList, -1, new Scalar(0,255,0));  
			System.Console.WriteLine ("DRAW CONTOURS DONE");


			return result;
		}*/

		public Mat CropText(Mat grey, int lowThresh, Mat img, int type){
			// from https://github.com/vipul-sharma20/document-scanner/blob/master/scanner.py
			Mat edges = createMat (img, type), blurred = createMat (img, type), heirarchy = createMat(img, type), orig = createMat(img, type);
			Java.Util.ArrayList javaList = new Java.Util.ArrayList ();
			List<MatOfPoint> contourList = new List<MatOfPoint>(),  targetList = new List<MatOfPoint>();
			List<MatOfPoint2f> contourList2f = new List<MatOfPoint2f>();
			MatOfPoint2f approx = new MatOfPoint2f(), target = new MatOfPoint2f();

			//edges.SetTo (new Scalar (0));

			grey.CopyTo (orig);
			System.Console.WriteLine ("DONE COPYING GREY TO ORIG");

			Imgproc.GaussianBlur(grey, blurred, new OpenCV.Core.Size(3, 3), 0);
			System.Console.WriteLine ("GAUSSIAN DONE");
			Imgproc.Canny (blurred, edges, 0, 50); //Imgproc.Canny (blurred, edges, lowThresh, lowThresh * 3);
			System.Console.WriteLine ("CANNY DONE");
			//grey.CopyTo (edges, blurred);
			//System.Console.WriteLine ("COPIED BLURRED TO EDGES");

			//return edges;

			Imgproc.FindContours (edges, contourList, heirarchy, Imgproc.RetrList, Imgproc.ChainApproxNone);//Imgproc.RetrExternal ?
			System.Console.WriteLine ("contourlist  count: "+contourList.Count.ToString ());
			System.Console.WriteLine ("FINDCONTOURS DONE");
			for(int i = 0; i < contourList.Count; i++){
				MatOfPoint2f myPt = new MatOfPoint2f();
				contourList.ElementAt(i).ConvertTo(myPt, CvType.Cv32fc2);//change? //type
				contourList2f.Add(myPt);
			}
			System.Console.WriteLine ("CONVERT TO MATOIPOINT2f DONE");
			contourList2f.OrderByDescending (x => Imgproc.ContourArea (x)).ToList ();

			//get approximate contour
			//might need to replace with a better find-largest-contour implenetation
			System.Console.WriteLine ("contourlist2f  count: "+contourList2f.Count.ToString ());
			foreach(MatOfPoint2f c in contourList2f){
				var p = Imgproc.ArcLength(c, true);
				Imgproc.ApproxPolyDP (c, approx, 0.02 * p, true);
				if (approx == null) {
					System.Console.WriteLine ("APPROX is null");
				}

				System.Console.WriteLine ("[APPROX ] "+approx.ToList ().Count.ToString ());

				if (approx.ToList ().Count == 4) {
					System.Console.WriteLine ("APPROX HAS 4 ELEMENTS");
					target = approx;
				}
			}
			System.Console.WriteLine ("GET APPROX CONTOUR DONE");
			//mapping target points to 800x800 quadrilateral?
			// from http://stackoverflow.com/questions/21084098/how-to-find-the-corners-of-a-rect-object-in-opencv
			//Imgproc.cvtColor(imgSource, imgSource, Imgproc.COLOR_BayerBG2RGB); //just in case (above link converts it back to color but ill try keeping it grey since i have further uses for the image)
			double[] temp_double = approx.Get(0,0);       
			System.Console.WriteLine ("temp_double size: "+approx.Get(0,0).ElementAt ((0)));
			OpenCV.Core.Point p1 = new OpenCV.Core.Point(temp_double[0], temp_double[1]);
			System.Console.WriteLine ("POINT (0,0) DONE");
			temp_double = approx.Get(1,0);    
			OpenCV.Core.Point p2 = new OpenCV.Core.Point(temp_double[0], temp_double[1]);
			System.Console.WriteLine ("POINT (1,0) DONE");
			temp_double = approx.Get(2,0);       
			OpenCV.Core.Point p3 = new OpenCV.Core.Point(temp_double[0], temp_double[1]);
			System.Console.WriteLine ("POINT (2,0) DONE");
			temp_double = approx.Get(3,0);       
			OpenCV.Core.Point p4 = new OpenCV.Core.Point(temp_double[0], temp_double[1]);
			System.Console.WriteLine ("POINT (3,0) DONE");
			List<OpenCV.Core.Point> source = new List<OpenCV.Core.Point>();
			source.Add(p1);
			source.Add(p2);
			source.Add(p3);
			source.Add(p4);
			System.Console.WriteLine ("SOURCELIST DONE");
			Mat startM = Converters.Vector_Point2f_to_Mat(source);//need to init startM?
			Mat result=warp(orig,startM, type);//either orig or grey //need to init result?
			System.Console.WriteLine ("WARP() DONE");
			//////////////////////////////////////////////////////////////////////////////////////////////////////
			System.Console.WriteLine ("TARGET POINTS MAPPED");

			MatOfPoint targetMatofPoint = new MatOfPoint();
			target.ConvertTo (targetMatofPoint, type);
			targetList.Add (targetMatofPoint);
			Imgproc.DrawContours (grey, targetList, -1, new Scalar(0,255,0));  
			System.Console.WriteLine ("DRAW CONTOURS DONE");
				
				
			return result;
		}

		public Mat warp(Mat inputMat,Mat startM, int type) {
			// from http://stackoverflow.com/questions/21084098/how-to-find-the-corners-of-a-rect-object-in-opencv
			int resultWidth = 1000;
			int resultHeight = 1000;

			Mat outputMat = new Mat(resultWidth, resultHeight, type);//?

			OpenCV.Core.Point ocvPOut1 = new OpenCV.Core.Point(0, 0);
			OpenCV.Core.Point ocvPOut2 = new OpenCV.Core.Point(0, resultHeight);
			OpenCV.Core.Point ocvPOut3 = new OpenCV.Core.Point(resultWidth, resultHeight);
			OpenCV.Core.Point ocvPOut4 = new OpenCV.Core.Point(resultWidth, 0);
			List<OpenCV.Core.Point> dest = new List<OpenCV.Core.Point>();
			dest.Add(ocvPOut1);
			dest.Add(ocvPOut2);
			dest.Add(ocvPOut3);
			dest.Add(ocvPOut4);
			Mat endM = Converters.Vector_Point2f_to_Mat (dest);      

			Mat perspectiveTransform = Imgproc.GetPerspectiveTransform(startM, endM);

			Imgproc.WarpPerspective(inputMat, 
				outputMat,
				perspectiveTransform,
				new OpenCV.Core.Size(resultWidth, resultHeight), 
				Imgproc.InterCubic);

			return outputMat;
		}

		public Mat UnSharpv1(Mat grey, Mat img, int type, double gausX, double gausY, double sigma){
			Mat lowContrastMask = createMat (img, type), blurred = createMat(img, type), kMaskProduct = createMat (img, type), sharp = createMat (img, type);

			//smooth/remove noise
			Imgproc.GaussianBlur(grey, blurred, new OpenCV.Core.Size(gausX, gausY), sigma);

			//mask
			OpenCV.Core.Core.Absdiff(grey, blurred, lowContrastMask);

			//sharpen weight
			OpenCV.Core.Core.Multiply(lowContrastMask, new OpenCV.Core.Scalar(Values.SHARPENMASKWEIGHT), kMaskProduct);

			//add kMaskProduct and original
			OpenCV.Core.Core.Add(grey, kMaskProduct, sharp);

			return sharp;
		}
		public Mat UnSharpFromDocs(Mat grey, Mat img, int type, double gausX, double gausY, double amount, double threshold, double sigma){
			//http://stackoverflow.com/questions/34770426/opencv-unsharp-mask-java-implementation
			Mat lowContrastMask = createMat (img, type), blurred = createMat(img, type), kMaskProduct = createMat (img, type), sharp = createMat (img, type);

			//smooth/remove noise
			Imgproc.GaussianBlur(grey, blurred, new OpenCV.Core.Size(gausX, gausY), sigma);

			//mask
			OpenCV.Core.Core.Absdiff(grey, blurred, lowContrastMask);

			//threshold
			Imgproc.Threshold(lowContrastMask, lowContrastMask, threshold, 0, Imgproc.ThreshBinaryInv);

			//add weighted
			OpenCV.Core.Core.AddWeighted(grey, 1+amount, blurred, -1*(amount), 0, sharp);

			//apply mask
			grey.CopyTo(sharp, lowContrastMask);

			return sharp;
		}
		public Mat UnSharpTest(Mat grey, Mat img, int type, double gausX, double gausY, double amount, double threshold, double sigma){
			Mat lowContrastMask = createMat (img, type), blurred = createMat(img, type), kMaskProduct = createMat (img, type), sharp = createMat (img, type);

			//smooth/remove noise
			Imgproc.GaussianBlur(grey, blurred, new OpenCV.Core.Size(gausX, gausY), sigma);

			//mask
			OpenCV.Core.Core.Absdiff(grey, blurred, lowContrastMask);

			Imgproc.Threshold(lowContrastMask, lowContrastMask, threshold, 0, Imgproc.ThreshBinaryInv);

			//sharpen weight
			OpenCV.Core.Core.Multiply(lowContrastMask, new OpenCV.Core.Scalar(amount), kMaskProduct);

			//add kMaskProduct and original
			OpenCV.Core.Core.Add(grey, kMaskProduct, sharp);

			return sharp;
		}
		public Mat BST(Mat grey, Mat img, int type, double gausX, double gausY){
			Mat mean = createMat(img, type), variance = createMat(img, type);

			Scalar scalar = OpenCV.Core.Core.Mean (grey);
			Scalar stdDev = scalar;//just to init
			//OpenCV.Core.Core.MeanStdDev(grey, grey, stdDev);
			//stdDev = stdDev*stdDev;


			return null;
		}
		public void Laplacian(){
			/*
				 * Laplacian attempt
				Imgproc.Laplacian(blurred, sharpened, type);
				OpenCV.Core.Core.AddWeighted(grey, 1+Values.LAPLACIANALPHABETA, sharpened, 1-Values.LAPLACIANALPHABETA, Values.SHARPENMASKWEIGHT, sharpWeighted);
				*/
		}
		public Mat createMat(Mat img, int type){
			return new Mat (img.Height (), img.Width (), type);
		}
		public void checkValue(Mat mat, string s){
			System.Console.WriteLine ("[Droid.Pic]: "+s+" - Width: " + mat.Size ().Width + " Height:"+mat.Size().Height);
		}

		public Stream SaturationTo0(Stream image){//rewrite using opencv
			//cant include byte-bitmap conversion in parent interface because Xamarin Forms contexts don't recognize Bitmap class
			return ToStream(BitmapToBytes (BitmapSaturationTo0 (BytesToBitmap (StreamToBytes(image)))));
		}
		public Stream ToStream(byte[] image){
			MemoryStream stream = new MemoryStream();
			stream.Write(image, 0, image.Length);
			stream.Seek(0, SeekOrigin.Begin);
			return stream;
		}
		public Bitmap BytesToBitmap(byte[] image){
			if (image == null)
				throw new NullReferenceException ("param of convertFromBytesToBitmap(byte[]) is null");
			return BitmapFactory.DecodeByteArray(image, 0, image.Length, new BitmapFactory.Options());
		}
		public byte[] BitmapToBytes(Bitmap bitmap){
			try{
				using (var stream = new MemoryStream())
				{
					bitmap.Compress(Bitmap.CompressFormat.Png, 100, stream);
					stream.Seek (0, SeekOrigin.Begin);
					return stream.ToArray();
				}
			}catch(Exception e){
				System.Console.WriteLine ("ERROR CONVERTING BITMAP TO BYTE " + e.Message);
			}
			return null;
		}
		public Bitmap BitmapSaturationTo0(Bitmap orginalBitmap) {
			try{
				ColorMatrix colorMatrix = new ColorMatrix();
				colorMatrix.SetSaturation (0.00f);

				ColorMatrixColorFilter colorMatrixFilter = new ColorMatrixColorFilter(
					colorMatrix);

				Bitmap blackAndWhiteBitmap = orginalBitmap.Copy(
					Bitmap.Config.Argb8888, true);

				Paint paint = new Paint();
				paint.SetColorFilter(colorMatrixFilter);

				Canvas canvas = new Canvas(blackAndWhiteBitmap);
				canvas.DrawBitmap(blackAndWhiteBitmap, 0, 0, paint);
				System.Console.WriteLine ("CONVERTED INTO BLACK AND WHITE IMAGE");

				return blackAndWhiteBitmap;
			}catch(Exception e){
				System.Console.WriteLine (e.Message);
			}
			return null;
		}
		public void SaveImageToDisk(Stream s, string filename)
		{
			if (s != null) {
				byte[] imageData = StreamToBytes(s);
				var dir = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDcim);
				var pictures = dir.AbsolutePath;
				//adding a time stamp time file name to allow saving more than one image... otherwise it overwrites the previous saved image of the same name
				//string name = filename + System.DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".jpg";
				string filePath = System.IO.Path.Combine(pictures, filename);
				global::Android.Net.Uri contentURI = global::Android.Net.Uri.FromFile (new Java.IO.File (filePath));
				try
				{
					System.IO.File.WriteAllBytes(filePath, imageData);
					//mediascan adds the saved image into the gallery
					var mediaScanIntent = new Intent(Intent.ActionMediaScannerScanFile);
					mediaScanIntent.SetData(contentURI);
					Xamarin.Forms.Forms.Context.SendBroadcast(mediaScanIntent);
					System.Console.WriteLine("IMAGE SAVED:"+ pictures);
				}
				catch(System.Exception e)
				{
					System.Console.WriteLine(e.Message);
				}
			}
		}
		public async Task<IEnumerable<Tesseract.Result>> loadFromPicDivideBy (Tesseract.PageIteratorLevel pageLevel, Stream s){
			TesseractApi api = new TesseractApi (Android.App.Application.Context, AssetsDeployment.OncePerVersion);
			System.Console.WriteLine("INIT TESSERACT -------------------------------------------------------------------------------------");

			try
			{
				if(await api.Init ("eng"))
				if(await api.SetImage (StreamToBytes(s)))
					imageResult = api.Results (pageLevel);

			}catch(Exception e){
				System.Console.WriteLine ("[Pics.loadFromPicDivideBy] Error loading picture: "+e.Message);
			}
			return null;
		}

		public static byte[] StreamToBytes(Stream input)
		{
			using (MemoryStream ms = new MemoryStream()){
				input.CopyTo(ms);
				ms.Seek(0, SeekOrigin.Begin);
				input.Seek (0, SeekOrigin.Begin);
				return ms.ToArray();
			}
		}
	}

	/*[Android.Runtime.Register("java/util/ArrayList", DoNotGenerateAcw=true)]
	public class ArrayList : AbstractList,	ISerializable, ICloneable, IRandomAccess, IDisposable{
		public override int Size(){
			return null;
		}
		public override Java.Lang.Object Get(int x){
			return null;
		}
		protected override Java.Lang.Object Clone(){
			return null;
		}
	}*/
}

