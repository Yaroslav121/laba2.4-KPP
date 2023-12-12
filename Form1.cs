using System;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        private Point robberPosition;
        private Point copPosition;
        private Point safeZone;
        private bool gameOver;
        private Random random = new Random();

        public Form1()
        {
            InitializeComponent();
            InitializeGame();
        }

        private void InitializeGame()
        {
            robberPosition = new Point(random.Next(ClientSize.Width - 10), random.Next(ClientSize.Height - 10));
            copPosition = new Point(ClientSize.Width - 60, ClientSize.Height - 60);

            // Генеруємо координати безпечної зони
            GenerateSafeZone();

            gameOver = false;

            // Запускаємо потік для руху розбійника
            Task.Run(() => MoveRobber());
        }

        private void GenerateSafeZone()
        {
            safeZone = new Point(random.Next(ClientSize.Width - 10), random.Next(ClientSize.Height - 10));
        }

        private void MoveRobber()
        {
            while (!gameOver)
            {
                Invoke(new Action(() =>
                {
                    // Логіка руху розбійника
                    int deltaX = (robberPosition.X < safeZone.X) ? 1 : -1; // Рух по X в напрямку безпечної зони
                    int deltaY = (robberPosition.Y < safeZone.Y) ? 1 : -1; // Рух по Y в напрямку безпечної зони

                    robberPosition.X += deltaX;
                    robberPosition.Y += deltaY;

                    // Перевірка на вихід за межі форми
                    if (robberPosition.X < 0)
                        robberPosition.X = ClientSize.Width - 10; // Розміщення розбійника з іншого боку по X
                    else if (robberPosition.X > ClientSize.Width - 10)
                        robberPosition.X = 0; // Розміщення розбійника з іншого боку по X

                    if (robberPosition.Y < 0)
                        robberPosition.Y = ClientSize.Height - 10; // Розміщення розбійника з іншого боку по Y
                    else if (robberPosition.Y > ClientSize.Height - 10)
                        robberPosition.Y = 0; // Розміщення розбійника з іншого боку по Y

                    // Перевірка на вихід за межі форми (вторинна)
                    if (robberPosition.X < 0 || robberPosition.X > ClientSize.Width - 10 ||
                        robberPosition.Y < 0 || robberPosition.Y > ClientSize.Height - 10)
                    {
                        gameOver = true;
                        MessageBox.Show("Гра завершена! Розбійник втік!");
                    }
                }));

                Thread.Sleep(100);
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (!gameOver)
            {
                // Логіка руху козака за допомогою стрілок
                if (e.KeyCode == Keys.Up && copPosition.Y > 0) copPosition.Y -= 10;
                else if (e.KeyCode == Keys.Down && copPosition.Y < ClientSize.Height - 10) copPosition.Y += 10;
                else if (e.KeyCode == Keys.Left && copPosition.X > 0) copPosition.X -= 10;
                else if (e.KeyCode == Keys.Right && copPosition.X < ClientSize.Width - 10) copPosition.X += 10;

                // Перевірка на зіткнення козака і розбійника 
                if (Math.Abs(robberPosition.X - copPosition.X) < 10 && Math.Abs(robberPosition.Y - copPosition.Y) < 10)
                {
                    gameOver = true;
                    MessageBox.Show("Гра завершена! Козак спіймав розбійника!");
                }

                // Перевірка, чи козак вже зайшов у безпечну зону
                if (copPosition == safeZone)
                {
                    gameOver = true;
                    MessageBox.Show("Гра завершена! Козак у безпечній зоні!");
                }

                // Перемалювання форми для відображення нового положення козака
                Invalidate();
            }
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            // Відображення розбійника, козака та безпечної зони на формі
            e.Graphics.FillRectangle(Brushes.Red, robberPosition.X, robberPosition.Y, 10, 10); // Розбійник
            e.Graphics.FillRectangle(Brushes.Blue, copPosition.X, copPosition.Y, 10, 10); // Козак
            e.Graphics.FillRectangle(Brushes.Green, safeZone.X, safeZone.Y, 10, 10); // Безпечна зона
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            KeyDown += Form1_KeyDown;
            Paint += Form1_Paint;
            Focus();
        }
    }
}
