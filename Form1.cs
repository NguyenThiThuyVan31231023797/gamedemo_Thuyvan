using System;
using System.CodeDom;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;
using System.IO;

namespace gamedemo
{
    public partial class Form1 : Form
    {
        private Timer gameTimer;
        private PictureBox trash;
        private int score = 0;
        private int highScore = 0;
        private int timeLeft = 60;
        private Random random;
        private string[] trashTypes = { "Chat long", "Thuc pham thua", "Kim loai", "Nhua tai che", "Giay", "Hop sua", "Rac thai con lai" };
        private string correctBin;
        //private Timer fallTimer = new Timer();

        public Form1()
        {
            InitializeComponent();
            LoadHighScore(); 
            InitializeGame();

        }
        private Dictionary<string, string> trashCategoryMapping;
        private void InitializeGame()
        {
            this.Text = "Trash Sorting Game";
            this.Size = new Size(800, 600);
            this.BackColor = Color.AliceBlue;

            random = new Random();

            //tạo nhãn cho điểm số, điểm số cao nhất (high score), và thanh thời gian
            Label lblTime = new Label
            {
                Text = $"Time: {timeLeft}s",
                Font = new Font("Arial", 16),
                Location = new Point(20, 20),
                AutoSize = true
            };
            lblTime.Name = "lblTime";
            this.Controls.Add(lblTime);


            Label lblScore = new Label
            {
                Text = $"Score: {score}",
                Font = new Font("Arial", 16),
                Location = new Point(20, 60),
                AutoSize = true
            };
            lblScore.Name = "lblScore";
            this.Controls.Add(lblScore);


            Label lblHighScore = new Label
            {
                //Text = $"High Score: {highScore}",
                Font = new Font("Arial", 16),
                Location = new Point(20, 100),
                AutoSize = true
            };
            this.Controls.Add(lblHighScore);

            //các loại rác và thùng rác tương ứng

            CreateTrashBin("chatlong", Properties.Resources.chatlong, new Point(20, 500)); 
            CreateTrashBin("thucphamthua", Properties.Resources.thucphamthua, new Point(240, 500)); 
            CreateTrashBin("kimloai", Properties.Resources.kimloai, new Point(460, 500)); 
            CreateTrashBin("nhuataiche", Properties.Resources.nhuataiche, new Point(680, 500));
            CreateTrashBin("giay", Properties.Resources.giay, new Point(900, 500));
            CreateTrashBin("hopsua", Properties.Resources.hopsua, new Point(1120, 500));
            CreateTrashBin("Racthaiconlai", Properties.Resources.racthaiconlai, new Point(1340, 500));


            trashCategoryMapping = new Dictionary<string, string>
            {
                {"chatlong1" , "chatlong"},
                {"giay1" , "giay" },
                {"giay2" , "giay" },
                {"giay3" , "giay" },
                {"giay4" , "giay" },
                {"hop1" , "hopsua" },
                {"hop2" , "hopsua" },
                {"hop3" , "hopsua" },
                {"hop4" , "hopsua" },
                {"huuco1" , "thucphamthua" },
                {"huuco2" , "thucphamthua" },
                {"huuco3" , "thucphamthua" },
                {"huuco4" , "thucphamthua" },
                {"huuco5" , "thucphamthua" },
                {"huuco6" , "thucphamthua" },
                {"kimloai1" , "kimloai" },
                {"kimloai2" , "kimloai" },
                {"kimloai3" , "kimloai" },
                {"kimloai4" , "kimloai" },
                {"kimloai5" , "kimloai" },
                {"kimloai6" , "kimloai" },
                {"kimloai7" , "kimloai" },
                {"kimloai8" , "kimloai" },
                {"nhua1" , "nhuataiche" },
                {"nhua2" , "nhuataiche" },
                {"nhua3" , "nhuataiche" },
                {"conlai1" , "racthaiconlai" },
                {"conlai2" , "racthaiconlai" },
                {"conlai3" , "racthaiconlai" },
                {"conlai4" , "racthaiconlai" },
                {"conlai5" , "racthaiconlai" },
                {"conlai6" , "racthaiconlai" },
                {"conlai7" , "racthaiconlai" },
                {"conlai8" , "racthaiconlai" },
                




            };


            // rác rơi
            trash = new PictureBox
            {
                Size = new Size(50, 50),
                Location = new Point(random.Next(100,600),50),
            };
            this.Controls.Add(trash);


            // đếm giờ
            gameTimer = new Timer
            {
                Interval = 1000
            };
            gameTimer.Tick += gameTimer_Tick;
            gameTimer.Start();

            this.KeyDown += MainForm_KeyDown;
            AssignNewTrash();




        }
        
        private Image ResizeImageMaintainAspect(Image img, int maxWidth, int maxHeight)
        {
            int originalWidth = img.Width;
            int originalHeight = img.Height;

            // Calculate the new dimensions while maintaining the aspect ratio
            float ratioX = (float)maxWidth / originalWidth;
            float ratioY = (float)maxHeight / originalHeight;
            float ratio = Math.Min(ratioX, ratioY);

            int newWidth = (int)(originalWidth * ratio);
            int newHeight = (int)(originalHeight * ratio);

            Bitmap resizedImg = new Bitmap(newWidth, newHeight);
            using (Graphics g = Graphics.FromImage(resizedImg))
            {
                g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

                g.DrawImage(img, 0, 0, newWidth, newHeight);
            }
            return resizedImg;
        }

        private void CreateTrashBin(string name, Image image, Point location)
        {
            PictureBox bin = new PictureBox
            {
                Size = new Size(200, 300),
                Location = location,
                Image = ResizeImageMaintainAspect(image, 200, 300),
                SizeMode = PictureBoxSizeMode.StretchImage,
                Tag = name.ToLower().Replace(" ", "")
            };

            this.Controls.Add(bin);
        }


        private void AssignNewTrash()
        {
            var randomTrash = trashCategoryMapping.ElementAt(random.Next(trashCategoryMapping.Count));
            string trashImageName = randomTrash.Key;
            correctBin = randomTrash.Value;

            Image trashImage = (Image)Properties.Resources.ResourceManager.GetObject(trashImageName);
            if (trashImage != null)
            {
                trash.Image = ResizeImageMaintainAspect(trashImage, 200, 200);
                trash.Size = new Size(200, 200);
                trash.SizeMode = PictureBoxSizeMode.StretchImage;
                trash.Left = random.Next(0, this.Width - trash.Width);


            }
            else
            {
                MessageBox.Show($"Trash image not found for: {trashImageName}");
                return;
            }
            trash.Left = random.Next(0, this.Width - trash.Width); // Vị trí ngang ngẫu nhiên
            trash.Top = 0; // Bắt đầu từ đỉnh màn hình
        }


        /*private void Form1_Load(object sender, EventArgs e)
        {
            fallTimer.Interval = 100; // 100ms cho mỗi lần Tick
            fallTimer.Tick += FallTimer_Tick;
            fallTimer.Start();
        }

        private void FallTimer_Tick(object sender, EventArgs e)
        {
            trash.Top += 10; // Rác rơi xuống 5 pixel mỗi lần Tick

            // Kiểm tra nếu rác rơi ra ngoài màn hình
            if (trash.Top > this.Height)
            {
                AssignNewTrash(); // Tạo rác mới
            }
        }*/

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.A && trash.Left >0)
            { 
                trash.Left -= 20;
            }
            else if (e.KeyCode == Keys.D && trash.Right < this.Width)
            {
                trash.Left += 20;

            }
            else if (e.KeyCode == Keys.S)
            {
                trash.Top += 20;
                if (trash.Bounds.Bottom >= this.Height - 200)
                {
                    foreach (Control ctrl in this.Controls)
                    {
                        if (ctrl is PictureBox bin && trash.Bounds.IntersectsWith(bin.Bounds))
                        {
                            CheckCorrectbin(bin.Tag.ToString());
                            break;
                        }
                    }
                    trash.Top = 50;
                    trash.Left = random.Next(100, 100);
                    AssignNewTrash();
                }
            }

        }



        private async void CheckCorrectbin(string bin)
        {
            PictureBox binControl = this.Controls.OfType<PictureBox>().FirstOrDefault(p => p.Tag.ToString() == bin);
            if (binControl != null)
            {
                // Debug: kiểm tra các giá trị đang so sánh
                Console.WriteLine($"Correct bin: {correctBin}, Selected bin: {bin}");

                if (string.Equals(bin, correctBin, StringComparison.OrdinalIgnoreCase))
                {
                    score += 10;
                    this.Controls["lblScore"].Text = $"Score: {score}";
                    binControl.BackColor = Color.LightGreen; // Highlight màu đúng
                }
                else
                {
                    binControl.BackColor = Color.IndianRed; // Highlight màu sai
                }

                await Task.Delay(500);
                binControl.BackColor = Color.Transparent; // Reset màu
            }
        }

        private void gameTimer_Tick(object sender, EventArgs e)
        {
            timeLeft--;
            this.Controls["lblTime"].Text = $"Time: {timeLeft}s";

            if(timeLeft <=0)
            {
                gameTimer.Stop();
                MessageBox.Show($"Time's up! Final Score: {score}");
                SaveHighScore();
                this.Close();

            }
        }

        




        private void LoadHighScore()
        {
            try
            {
                if (File.Exists("highscore.txt"))
                {
                    highScore = int.Parse(File.ReadAllText("highscore.txt"));

                }
            

            }
            catch
            {
                highScore = 0;
            }
        }

        private void SaveHighScore()
        {
            if (score>highScore)
            {
                File.WriteAllText("highscore.txt", score.ToString());
            }
        }
    }
}
