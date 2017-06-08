using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.EnterpriseServices;
using System.IO;
using System.Linq;

namespace NoiRLibrary
{
    public class ImageLibrary : ServicedComponent
    {
        public int GetOrientation(string readPath)
        {
            FileStream stream = new FileStream(readPath, FileMode.Open, FileAccess.Read);

            Image imageFile = Image.FromStream(stream, false, false);

            int orientationId = 0x0112;
            int orientation = 1;

            if (imageFile.PropertyIdList.Contains(orientationId))
            {
                PropertyItem item = imageFile.GetPropertyItem(orientationId);

                orientation = item.Value[0];
            }

            stream.Dispose();

            return orientation;
        }

        /// <summary>
        /// 비율 유지하며 이미지 크기 조절하여 다른 이름으로 저장
        /// </summary>
        /// <param name="readPath">원본 이미지의 경로.</param> 
        /// <param name="savePath">이미지가 저장 될 경로.</param> 
        /// <param name="width">이미지 폭.</param>
        /// <param name="height">이미지 높이.</param>
        /// <param name="fill">남는 영역 채우기.</param>
        public void Resize(string readPath, string savePath, int width, int height, bool fill)
        {
            Image imageFile = Image.FromFile(readPath, true); //이미지 파일

            int fileWidth = imageFile.Width;
            int fileHeight = imageFile.Height;

            int fileX = 0;
            int fileY = 0;
            int applyX = 0;
            int applyY = 0;

            float percent = 0;
            float percentWidth = 0;
            float percentHeight = 0;

            percentWidth = ((float)width / (float)fileWidth); //폭 비율
            percentHeight = ((float)height / (float)fileHeight); //높이 비율

            //채우기를 사용하지 않을 경우
            if (!fill)
            {
                if (percentHeight < percentWidth)
                {
                    percent = percentHeight;
                }
                else
                {
                    percent = percentWidth;
                }
            }
            else
            {
                if (percentHeight > percentWidth)
                {
                    percent = percentHeight;

                    applyX = (int)Math.Round((width - (fileWidth * percent)) / 2);
                }
                else
                {
                    percent = percentWidth;

                    applyY = (int)Math.Round((height - (fileHeight * percent)) / 2);
                }
            }

            //최소 비율
            if (percent > 1)
                percent = 1;

            int applyWidth = (int)Math.Round(fileWidth * percent); //적용 할 폭
            int applyHeight = (int)Math.Round(fileHeight * percent); //적용 할 넓이

            //비트맵 이미지 생성
            Bitmap bitmapImage = new Bitmap(
                applyWidth <= width ? applyWidth : width,
                applyHeight < height ? applyHeight : height,
                PixelFormat.Format32bppRgb);

            //채우기 생성
            Graphics graphicsImage = Graphics.FromImage(bitmapImage);

            graphicsImage.Clear(Color.White);

            graphicsImage.InterpolationMode = InterpolationMode.Default;
            graphicsImage.CompositingQuality = CompositingQuality.HighQuality;
            graphicsImage.SmoothingMode = SmoothingMode.HighQuality;

            graphicsImage.DrawImage(imageFile,
                new Rectangle(applyX, applyY, applyWidth, applyHeight),
                new Rectangle(fileX, fileY, fileWidth, fileHeight),
                GraphicsUnit.Pixel);

            bitmapImage.Save(savePath);

            graphicsImage.Dispose();
        }

        /// <summary> 
        /// 이미지 화질을 변경하여 다른 이름으로 저장
        /// </summary> 
        /// <param name="readPath">원본 이미지의 경로.</param> 
        /// <param name="savePath">이미지가 저장 될 경로.</param> 
        /// <param name="quality">이미지 화질.</param> 
        public void ReduceQuality(string readPath, string savePath, int quality)
        {
            Image imageFile = Image.FromFile(readPath, true);

            if (quality < 0 || quality > 100) {
                quality = 100;
            }

            // 이미지 화질을 위한 인코더 매개변수
            EncoderParameter qualityParam = new EncoderParameter(Encoder.Quality, quality);

            // JPEG 이미지 코덱 
            ImageCodecInfo codec = GetCodec("image/jpeg");

            EncoderParameters encoderParams = new EncoderParameters(1);

            encoderParams.Param[0] = qualityParam;

            imageFile.Save(savePath, codec, encoderParams);
        }

        /// <summary>
        /// 이미지 코덱 가져오기
        /// </summary>
        /// <param name="mimeType">MIME 형식.</param>
        /// <returns></returns>
        private ImageCodecInfo GetCodec(string mimeType)
        {
            // Get image codecs for all image formats 
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();

            // Find the correct image codec 
            for (int i = 0; i < codecs.Length; i++)
                if (codecs[i].MimeType == mimeType)
                    return codecs[i];
        
            return null;
        }
    }
}
