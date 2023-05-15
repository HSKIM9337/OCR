using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Tesseract;

namespace OCR.ViewModels
{
    public class MainWindowViewModel : ObservableRecipient
    {

        #region Private Methods

        private void OnGetImagePath()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.InitialDirectory = @"C:\";
            dialog.Filter = "Image Files (*.jpg;*.jpeg;*.png;*.bmp)|*.jpg;*.jpeg;*.png;*.bmp";

            if (dialog.ShowDialog() == true)
            {
                ImgPath = dialog.FileName;
                MessageBox.Show(ImgPath);
            }
            else
            {
                MessageBox.Show("취소하셨습니다");
            }

        }

        private void OCR()
        {
            using (var engine = new TesseractEngine("./tessdata", "eng", EngineMode.Default))
            {
                using (var img = Pix.LoadFromFile(ImgPath))
                {
                    using (var page = engine.Process(img))
                    {
                        this.OcrResult = page.GetText();
                        MessageBox.Show(OcrResult);
                    }
                }
            }
        }

        private void OnUsingPaPago()
        {
            string sUrl = "https://openapi.naver.com/v1/papago/n2mt";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(sUrl);
            request.Headers.Add("X-Naver-Client-Id", "7eQRHWNSVHWxknRJ3bX8");
            request.Headers.Add("X-Naver-Client-Secret", "S2BpV7vAjq");
            request.Method = "POST";

            this._ocrResult = _ocrResult.Replace("\n", "");

            byte[] bytearry = Encoding.UTF8.GetBytes("source=en&target=ko&text=" + this.OcrResult);
            request.ContentType = "application/x-www-form-urlencoded";

            request.ContentLength = bytearry.Length;

            Stream st = request.GetRequestStream();
            st.Write(bytearry, 0, bytearry.Length);
            st.Close();

            // 응답 데이터 가져오기 (출력포맷)
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream stream = response.GetResponseStream();
            StreamReader reader = new StreamReader(stream, Encoding.UTF8);
            string text = reader.ReadToEnd();
            //MessageBox.Show(text);
            stream.Close();
            response.Close();
            reader.Close();

            JObject jObject = JObject.Parse(text);
            //MessageBox.Show(this.OcrResult);

            this.TransalteResult = jObject["message"]["result"]["translatedText"].ToString();

            //MessageBox.Show(TransalteResult);
        }




        #endregion Private Methods

        #region Public Properties
        private String _translateResult;
        public String TransalteResult
        {
            get => _translateResult;
            set => SetProperty(ref _translateResult, value);
        }

        private String _ocrResult;
        public String OcrResult
        {
            get => _ocrResult;
            set => SetProperty(ref _ocrResult, value);
        }

        private String _imgPath;
        public String ImgPath
        {
            get => _imgPath;
            set => SetProperty(ref _imgPath, value);
        }

        private BitmapImage _image;
        public BitmapImage Image
        {
            get => _image;
            set => SetProperty(ref _image, value);
        }

        #endregion Public Properties

        #region Public Methods

        public ICommand GetImage { get; set; }
        public ICommand Translate { get; set; }
        public ICommand ShowResult { get; set; }

        #endregion Public Methods

        #region Constructor
        public MainWindowViewModel()
        {
            this.GetImage = new RelayCommand(OnGetImagePath);
            this.ShowResult = new RelayCommand(OCR);
            this.Translate = new RelayCommand(OnUsingPaPago);
        }
        #endregion Constructor
    }
}
