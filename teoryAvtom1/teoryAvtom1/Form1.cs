using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace teoryAvtom1
{
    public partial class Form1 : Form
    {

        private GameState gameState = new GameState();
        private List<ProgressBar> boxProgressBars = new List<ProgressBar>();
        private List<Label> boxLabels = new List<Label>();
        private PictureBox currentDetailPictureBox;

        public Form1()
        {
            InitializeComponent();
            //ColorSettingsForm.BaseSelectionColor();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // 2. Затем создаем PictureBox для детали 
            InitializeDetailPictureBox();
            // 1. Сначала загружаем ВСЕ фоновые спрайты
            LoadSprites();
            // 3. Потом остальная инициализация
           InitializeBoxLabels();
           InitializeProgressBars();

           currentDetailLabel.Text = "Конвейер пуст";
        }


        private void InitializeBoxLabels()
        {
            boxLabels.Add(labelBox1);
            boxLabels.Add(labelBox2);
            boxLabels.Add(labelBox3);
            boxLabels.Add(labelBox4);
            boxLabels.Add(labelBox5);
            boxLabels.Add(labelBox6);
            boxLabels.Add(labelBox7);
            boxLabels.Add(labelBox8);
        }

        private void LoadSprites()
        {
            try
            {
                conveyorPictureBox.Image = Image.FromFile("Sprites/conveyor.png");
                conveyorPictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
                conveyorPictureBox.BackColor = Color.Transparent;

                generatorPictureBox.Image = Image.FromFile("Sprites/generator.png");
                generatorPictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
                generatorPictureBox.BackColor = Color.Transparent;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки спрайтов: {ex.Message}");
            }
        }

        private void InitializeProgressBars()
        {
            boxProgressBars.Add(progressBar1);
            boxProgressBars.Add(progressBar2);
            boxProgressBars.Add(progressBar3);
            boxProgressBars.Add(progressBar4);
            boxProgressBars.Add(progressBar5);
            boxProgressBars.Add(progressBar6);
            boxProgressBars.Add(progressBar7);
            boxProgressBars.Add(progressBar8);

            foreach (var pb in boxProgressBars)
            {
                pb.Maximum = 8;
                pb.Value = 0;
            }
        }

        private void InitializeDetailPictureBox()
        {
            currentDetailPictureBox = new PictureBox();
            currentDetailPictureBox.Size = new Size(40, 40);
            currentDetailPictureBox.BackColor = Color.Transparent;
            currentDetailPictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
            currentDetailPictureBox.Visible = false;
            this.Controls.Add(currentDetailPictureBox);
            currentDetailPictureBox.BringToFront();
        }

        private void startButton_Click(object sender, EventArgs e)
        {
            if (gameTimer.Enabled)
            {
                // Пауза - останавливаем только таймер
                gameTimer.Stop();
                startButton.Text = "Старт";
            }
            else
            {
                // Продолжение - проверяем что игра настроена
                if (gameState.AllowedTypes == null || gameState.AllowedColors == null ||
                    gameState.AllowedTypes.Count != 4 || gameState.AllowedColors.Count != 2)
                {
                    MessageBox.Show("Сначала выберите настройки через меню!\n(4 типа деталей и 2 цвета)");
                    return;
                }

                // Если игра еще не запускалась - инициализируем
                if (!gameState.IsRunning && gameState.Boxes.Count == 0)
                {
                    gameState.CreateBoxes();
                    gameState.IsRunning = true;
                    gameState.GenerateSingleDetail();
                    UpdateBoxesDisplay();
                }

                // Запускаем таймер (продолжаем игру)
                gameTimer.Start();
                startButton.Text = "Стоп";
                UpdateUI();
            }
        }


        private void gameTimer_Tick(object sender, EventArgs e)
        {
            if (gameState.IsRunning)
            {
                gameState.MoveConveyor(); // Вся логика теперь в GameState
                UpdateUI();

                if (!gameState.IsRunning)
                {
                    gameTimer.Stop();
                    MessageBox.Show("Все ящики заполнены! Игра окончена.");
                }
            }
        }

        private void UpdateUI()
        {
            // Обновление движущейся детали
            UpdateCurrentDetail();

            // Обновление ящиков
            UpdateBoxesDisplay();
        }

        private void UpdateCurrentDetail()
        {
            if (gameState.CurrentDetail != null)
            {
                currentDetailLabel.Text = $"{gameState.CurrentDetail} | Позиция: {gameState.CurrentDetailPosition}%";

                // Позиционирование детали над конвейером
                int xPos = conveyorPictureBox.Left +
                          (conveyorPictureBox.Width * gameState.CurrentDetailPosition / GameState.ConveyorLength);
                int yPos = conveyorPictureBox.Top - 10;

                currentDetailPictureBox.Location = new Point(xPos, yPos);
                currentDetailPictureBox.Visible = true;

                // Загрузка спрайта детали
                string detailPath = Path.Combine("Sprites",
                    $"detail_{gameState.CurrentDetail.Type}_{gameState.CurrentDetail.Color}.png");
                if (File.Exists(detailPath))
                {
                    currentDetailPictureBox.Image = Image.FromFile(detailPath);
                }
            }
            else
            {
                currentDetailLabel.Text = "Конвейер пуст";
                currentDetailPictureBox.Visible = false;
            }
        }

        private void UpdateBoxesDisplay()
        {
            for (int i = 0; i < gameState.Boxes.Count && i < boxProgressBars.Count; i++)
            {
                var box = gameState.Boxes[i];
                boxProgressBars[i].Value = box.CurrentCount;

                if (i < boxLabels.Count)
                {
                    boxLabels[i].Text = $"{box.DisplayName}\n{box.CurrentCount}/8";
                    boxLabels[i].ForeColor = box.IsFull ? Color.Green :
                                           !box.IsActive ? Color.Gray : Color.Black;
                }
                // ЗАГРУЖАЕМ СПРАЙТ ЯЩИКА как у детали
                string boxPath = Path.Combine("Sprites",
                    $"box_{box.TargetType}_{box.TargetColor}.png");
                if (File.Exists(boxPath))
                {
                    // Находим соответствующий PictureBox для ящика
                    var boxPictureBox = Controls.Find($"pictureBox{i + 1}", true).FirstOrDefault() as PictureBox;
                    if (boxPictureBox != null)
                    {
                        boxPictureBox.Image = Image.FromFile(boxPath);
                        boxPictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
                    }
                }
            }
        }

        // Меню настроек
        private void settingsDetailsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var detailsForm = new DetailSettingsForm();
            if (detailsForm.ShowDialog() == DialogResult.OK)
            {
                gameState.AllowedTypes = detailsForm.SelectedTypes;
                MessageBox.Show($"Выбраны типы: {string.Join(", ", gameState.AllowedTypes)}");
            }
        }

        private void settingsColorsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var colorsForm = new ColorSettingsForm();
            if (colorsForm.ShowDialog() == DialogResult.OK)
            {
                gameState.AllowedColors = colorsForm.SelectedColors;
                MessageBox.Show($"Выбраны цвета: {string.Join(", ", gameState.AllowedColors)}");
            }
        }

        // Остальные методы меню
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Функция 'Открыть' будет реализована позже.");
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Функция 'Сохранить' будет реализована позже.");
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void helpAboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Конвейерный сортировщик деталей. Версия 1.0");
        }

        private void helpAuthorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("ФАВТ, 2 курс, Дмитрий Серебренников, 2025");
        }

        // Пустые обработчики
        private void fileToolStripMenuItem_Click(object sender, EventArgs e) { }
        private void settingsToolStripMenuItem_Click(object sender, EventArgs e) { }
        private void spravkaToolStripMenuItem_Click(object sender, EventArgs e) { }

        private void resetButton_Click(object sender, EventArgs e)
        {
            //gameTimer.Stop();
            gameState.ResetGame();
            startButton.Text = "Старт";

            // Сбрасываем прогресс-бары
            foreach (var pb in boxProgressBars)
            {
                pb.Value = 0;
            }

            // Сбрасываем надписи
            foreach (var label in boxLabels)
            {
                label.Text = "";
                label.ForeColor = Color.Black;
            }

            currentDetailLabel.Text = "Конвейер пуст";
            currentDetailPictureBox.Visible = false;
            gameState.CreateBoxes();
            gameState.IsRunning = true;
            gameState.GenerateSingleDetail();
            UpdateBoxesDisplay();

            gameTimer.Start();
            startButton.Text = "Стоп";
            UpdateUI();
        }
    }
}