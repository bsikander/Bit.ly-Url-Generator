using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Net;
using System.Xml;
using System.Configuration;
using System.Reflection;
using System.Security.AccessControl;


namespace odeskProject
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            textBox2.Text = System.Configuration.ConfigurationSettings.AppSettings["Login"];
            textBox3.Text = System.Configuration.ConfigurationSettings.AppSettings["Keys"];

            pBar.Step = 1;

            pBar.Minimum = 1;
            
        }
        String ImageFileName = String.Empty;
        String firstPart = String.Empty;
        String secondPart = String.Empty;
        String extension = String.Empty;
        


        private void button1_Click(object sender, EventArgs e)
        {
            String input = String.Empty;
            openFileDialog1.Filter = "Image Files|*.jpg;*.gif;*.bmp;*.png;*.jpeg|All Files|*.*";
            openFileDialog1.InitialDirectory = @"C:\";
            openFileDialog1.FilterIndex = 1;
            openFileDialog1.Title = "Open an Image File";

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Image_Path.Text = openFileDialog1.FileName;

                

                pictureBox1.Image = Image.FromFile(openFileDialog1.FileName);
                pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;

                //System.IO.Path.GetRandomFileName();

                

            }
        }

        private void generateRandomFileNames()
        {
            string randomFileName = System.IO.Path.GetRandomFileName();
            string fileNameToCutHalf=  randomFileName.Split('.')[0];
            int lenghtOfString=fileNameToCutHalf.Length;



            firstPart = fileNameToCutHalf.Substring(0, lenghtOfString / 2);
            secondPart = fileNameToCutHalf.Substring((lenghtOfString / 2) + 1, lenghtOfString - ((lenghtOfString / 2) + 1));
            
        }

        private void processFileNameToExtractFileName(string fileName)
        {
            string name;
            int index = fileName.LastIndexOf("\\");
            name = fileName.Substring(index + 1);

            ImageFileName= name.Split('.')[0];
            extension = name.Split('.')[1];


        }

        private void button2_Click(object sender, EventArgs e)
        {
            pBar.Value = 1;

            float percentageVar = (float)0.5;

            processFileNameToExtractFileName(openFileDialog1.FileName);
            int variation = Int32.Parse( txt_count.Text);

            string generatedFileName = string.Empty;
            DirectoryInfo pathOfDesktop=null;
            string pathOfImageFolder = string.Empty;

            pBar.Maximum = variation;

            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            if (!(System.IO.Directory.Exists(desktopPath + @"\Images")))
            {
                //System.Security.AccessControl.DirectorySecurity abc = new System.Security.AccessControl.DirectorySecurity();
                //System.Security.AccessControl.FileSystemAccessRule abb=new FileSystemAccessRule(
                //abc.SetAccessRule = new 
                pathOfDesktop = System.IO.Directory.CreateDirectory(desktopPath + @"\Images");
            }
            else
            {
                pathOfImageFolder = desktopPath + @"\Images";
                //pathOfDesktop=(DirectoryInfo)(desktopPath + @"\Images");
            }

            FileStream aFile = new FileStream(@"D:\Generated Names.txt", FileMode.OpenOrCreate);
            aFile.Close();
            TextWriter tsw = new StreamWriter(@"D:\Generated Names.txt"); ;

            for (int i = 0; i < variation; i++)
            {
                generateRandomFileNames();

                generatedFileName = firstPart + txt_filename.Text + secondPart +"."+ extension;

                //Writing text to the file.
                Image a = Image.FromFile(openFileDialog1.FileName);
                Image test = Resize(a, (float)percentageVar);

                if(pathOfDesktop != null)
                    test.Save(pathOfDesktop.FullName +"\\"+ generatedFileName);
                else
                    test.Save(pathOfImageFolder + "\\" + generatedFileName);
                

                tsw.WriteLine(generatedFileName);

                //Close the file.

                percentageVar += (float)0.01;

                pBar.PerformStep();

            }
            tsw.Close();
            //label14.Text = "Status : Done";
            toolStripStatusLabel1.Text = "File Successfully written";


            FolderZipper.ZipUtil.ZipFiles(desktopPath + @"\Images", "Images.rar", desktopPath, "");
           
            
            
            //File.SetAttributes(desktopPath + @"\Images", File.GetAttributes(desktopPath + @"\Images") & ~(FileAttributes.Archive | FileAttributes.ReadOnly));
            //File.Delete(desktopPath + @"\Images");

            //FileSystemInfo aa = new FileInfo(desktopPath + @"\Images");
            //string pathofFile = desktopPath + @"\Images";
            //giveRights(pathofFile);

            //DeleteFileSystemInfo(aa);
        }

        public static void AddDirectorySecurity(string FileName, string Account, FileSystemRights Rights,
                                               InheritanceFlags Inheritance, PropagationFlags Propogation,
                                               AccessControlType ControlType)
        {
            // Create a new DirectoryInfo object. 
            DirectoryInfo dInfo = new DirectoryInfo(FileName);
            // Get a DirectorySecurity object that represents the  
            // current security settings. 
            DirectorySecurity dSecurity = dInfo.GetAccessControl();
            // Add the FileSystemAccessRule to the security settings.  
            dSecurity.AddAccessRule(new FileSystemAccessRule(Account,
                                                             Rights,
                                                             Inheritance,
                                                             Propogation,
                                                             ControlType));
            // Set the new access settings. 
            dInfo.SetAccessControl(dSecurity);
        } 


        private static void DeleteFileSystemInfo(FileSystemInfo fsi)
        {
            fsi.Attributes = FileAttributes.Normal;
            var di = fsi as DirectoryInfo;

            if (di != null)
            {
                foreach (var dirInfo in di.GetFileSystemInfos())
                    DeleteFileSystemInfo(dirInfo);
            }

            fsi.Delete();
        }

        private void giveRights(string pathOfFile)
        {

        DirectoryInfo myDirectoryInfo = new DirectoryInfo(pathOfFile);
     
               // Get a DirectorySecurity object that represents the 
                // current security settings.
                DirectorySecurity myDirectorySecurity = myDirectoryInfo.GetAccessControl();
                string User = System.Environment.UserDomainName; 
   
              myDirectorySecurity.AddAccessRule(new FileSystemAccessRule(User, 
                                              FileSystemRights.FullControl, AccessControlType.Allow));
     
              // Set the new access settings. 
              myDirectoryInfo.SetAccessControl(myDirectorySecurity);
     
             // Showing a Succesfully Done Message
             
    }


        /// <summary>
        /// method for resizing an image
        /// </summary>
        /// <param name="img">the image to resize</param>
        /// <param name="percentage">Percentage of change (i.e for 105% of the original provide 105)</param>
        /// <returns></returns>
        public Image Resize(Image img, float percentage)
        {
            //get the height and width of the image
            int originalW = img.Width;
            int originalH = img.Height;

            //get the new size based on the percentage change
            int resizedW = (int)(originalW * percentage);
            int resizedH = (int)(originalH * percentage);

            //create a new Bitmap the size of the new image
            Bitmap bmp = new Bitmap(resizedW, resizedH);
            //create a new graphic from the Bitmap
            Graphics graphic = Graphics.FromImage((Image)bmp);
            graphic.InterpolationMode = InterpolationMode.HighQualityBicubic;
            //draw the newly resized image
            graphic.DrawImage(img, 0, 0, resizedW, resizedH);
            //dispose and free up the resources
            graphic.Dispose();
            
            //return the image
            return (Image)bmp;
        }

        string Shorten(string url, string login, string key, bool xml)
        {

            url = Uri.EscapeUriString(url);
            string reqUri =String.Format("http://api.bit.ly/v3/shorten?" +
            "login={0}&apiKey={1}&format={2}&longUrl={3}",
            login, key, xml ? "xml" : "txt", url);             

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(reqUri);

            req.Timeout = 10000; // 10 seconds
            // if the function fails and format==txt throws an exception
            Stream stm = req.GetResponse().GetResponseStream();
            if (xml)
            {
                XmlDocument doc = new XmlDocument();

                doc.Load(stm);
            // error checking for xml

                if (doc["response"]["status_code"].InnerText != "200")
                    throw new WebException(doc["response"]["status_txt"].InnerText);

            return doc["response"]["data"]["url"].InnerText;

            }
            else // Text
                using (StreamReader reader = new StreamReader(stm))
            return reader.ReadLine();

}

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            pBar.Value = 1;
            int count = Int32.Parse(textbox.Text);

            pBar.Maximum = count;

            DirectoryInfo pathOfDesktop = null;
            string pathOfImageFolder = string.Empty;

            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            FileStream aFile = new FileStream(desktopPath + @"\BitLy URLs.txt", FileMode.OpenOrCreate);
            aFile.Close();
            TextWriter tsw = new StreamWriter(desktopPath + @"\BitLy URLs.txt");

            string imageUrl = string.Empty;
            string bitlyLinkURL = string.Empty;
            string bitlyImageURL = string.Empty;
            string linkUrl = string.Empty;

            for (int j = 0; j < count; j++)
            {
                linkUrl = generateRandomURLs(txt_url.Text, comboBox1.SelectedIndex + 1);

                if (checkBox1.Checked == false)
                {
                    imageUrl = generateRandomURLs(txt_image.Text, comboBox1.SelectedIndex + 1);
                    bitlyLinkURL = Shorten(linkUrl, textBox2.Text, textBox3.Text, true);
                    bitlyImageURL = Shorten(imageUrl, textBox2.Text, textBox3.Text, true);
                }
                else
                {
                    //int indexOfurl = txt_image.Text.IndexOf('.');
                    //string tfinalURL = "http://" + txt_image.Text.Substring(indexOfurl + 1) ;
                    //imageUrl = tfinalURL;
                    bitlyLinkURL = Shorten(linkUrl, textBox2.Text, textBox3.Text, true);
                    bitlyImageURL = txt_image.Text;

                }

                tsw.WriteLine("<div class=\"fullP\">" + "<a href=" + bitlyLinkURL + " target=\"_blank\">" + "<img src=" + bitlyImageURL + "/>" + "</a>" + "</div>");
                //tsw.WriteLine("<a href=" + linkUrl + " target=\"_blank\">");
                //tsw.WriteLine("<img src=" + imageUrl + "/>");
                //tsw.WriteLine("</a>");
                //tsw.WriteLine("</div>");
                //tsw.WriteLine("");

                pBar.PerformStep();
                
            }
            tsw.Close();
            //label14.Text = "Status : Done";
            toolStripStatusLabel1.Text = "File Successfully written";
            
        }

        private string generateRandomURLs(string url , int type)
        {
            string randomFileName = System.IO.Path.GetRandomFileName();
            string fileNameToCutHalf = randomFileName.Split('.')[0];
            int lenghtOfString = fileNameToCutHalf.Length;

            string finalURL= string.Empty;
            int indexOfurl = 0;

            if (type == 1)
            {
                indexOfurl= url.IndexOf('.');
                finalURL = "http://" + fileNameToCutHalf + url.Substring(indexOfurl); 
               // finalURL = 
            }
            else if(type == 2) // extension
            {
                indexOfurl = url.IndexOf('.');
                finalURL = "http://" + url.Substring(indexOfurl+1) + "/" + fileNameToCutHalf;
            }
            return finalURL;
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

           
//FileStream confFile = new FileStream(@"D:\Configuration.txt", FileMode.Append);
            //confFile.Close();
            //TextWriter tswConf;//= new StreamWriter(@"D:\Configuration.txt");
            //tswConf = File.AppendText(@"D:\Configuration.txt");

            //tswConf.WriteLine("Login:" + textBox2.Text);
            //tswConf.Close();

            WriteSetting("Login", textBox2.Text);

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            //FileStream confFile = new FileStream(@"D:\Configuration.txt", FileMode.Append);
            //confFile.Close();
            //TextWriter tswConf; // = new StreamWriter(@"D:\Configuration.txt", File.AppendText(@"D:\Configuration.txt"));
            //tswConf = File.AppendText(@"D:\Configuration.txt");

            //tswConf.WriteLine("Key;" + textBox3.Text);
            //tswConf.Close();
            WriteSetting("Keys", textBox3.Text);
        }

        private static XmlDocument loadConfigDocument()
        {
            XmlDocument doc = null;
            try
            {
                doc = new XmlDocument();
                doc.Load(getConfigFilePath());
                return doc;
            }
            catch (System.IO.FileNotFoundException e)
            {
                throw new Exception("No configuration file found.", e);
            }
        }

        private static string getConfigFilePath()
        {
            return Assembly.GetExecutingAssembly().Location + ".config";
        }

        public static void WriteSetting(string key, string value)
        {
            // load config document for current assembly
            XmlDocument doc = loadConfigDocument();

            // retrieve appSettings node
            XmlNode node = doc.SelectSingleNode("//appSettings");

            if (node == null)
                throw new InvalidOperationException("appSettings section not found in config file.");

            try
            {
                // select the 'add' element that contains the key
                XmlElement elem = (XmlElement)node.SelectSingleNode(string.Format("//add[@key='{0}']", key));

                if (elem != null)
                {
                    // add value for key
                    elem.SetAttribute("value", value);
                }
                else
                {
                    // key was not found so create the 'add' element 
                    // and set it's key/value attributes 
                    elem = doc.CreateElement("add");
                    elem.SetAttribute("key", key);
                    elem.SetAttribute("value", value);
                    node.AppendChild(elem);
                }
                doc.Save(getConfigFilePath());
            }
            catch
            {
                throw;
            }
        }

    }
}
