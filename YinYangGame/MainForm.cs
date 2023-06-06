using System;
using System.Drawing;
using System.Windows.Forms;

namespace YinYangGame
{
    public partial class MainForm : Form
    {
        private const int CellSize = 20;
        private const int GridWidth = 30;
        private const int GridHeight = 30;
        private Button startButton;
        private Button stopButton;
        private Timer timer;
        private Panel gridPanel;
        private CellState[,] grid;
        private Random random;

        public MainForm()
        {
            InitializeComponent();
            InitializeComponents();
            random = new Random();
            InitializeGrid();
        }

        private void InitializeComponents()
        {
            // Создание и настройка кнопки "Старт"
            startButton = new Button();
            startButton.Text = "Старт";
            startButton.BackColor = Color.Black;
            startButton.ForeColor = Color.White;
            startButton.Click += startButton_Click;

            // Создание и настройка кнопки "Стоп"
            stopButton = new Button();
            stopButton.Text = "Стоп";
            stopButton.BackColor = Color.Black;
            stopButton.ForeColor = Color.White;
            stopButton.Click += stopButton_Click;

            // Создание и настройка таймера
            timer = new Timer();
            timer.Interval = 1000; // Интервал в миллисекундах (здесь 1 секунда)
            timer.Tick += timer_Tick;

            // Создание и настройка панели отображения
            gridPanel = new Panel();
            gridPanel.Size = new Size(600, 600);
            gridPanel.BackColor = Color.White;
            gridPanel.MouseClick += MainForm_MouseClick;
            gridPanel.Paint += MainForm_Paint;

            // Добавление элементов на форму
            Controls.Add(startButton);
            Controls.Add(stopButton);
            Controls.Add(gridPanel);

            // Расположение элементов на форме
            startButton.Location = new Point(10, 10);
            stopButton.Location = new Point(90, 10);
            gridPanel.Location = new Point(10, 50);

            // Расчет размера формы
            int formWidth = gridPanel.Right + 20;
            int formHeight = gridPanel.Bottom + 20;
            Size = new Size(formWidth, formHeight);
        }

        private void InitializeGrid()
        {
            grid = new CellState[GridWidth, GridHeight];
            for (int i = 0; i < GridWidth; i++)
            {
                for (int j = 0; j < GridHeight; j++)
                {
                    grid[i, j] = random.Next(0, 2) == 0 ? CellState.Dead : (random.Next(0, 2) == 0 ? CellState.Yin : CellState.Yang);
                }
            }
        }

        private void MainForm_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            for (int i = 0; i < GridWidth; i++)
            {
                for (int j = 0; j < GridHeight; j++)
                {
                    Brush brush;
                    switch (grid[i, j])
                    {
                        case CellState.Dead:
                            brush = Brushes.LightGray;
                            break;
                        case CellState.Yin:
                            brush = Brushes.Black;
                            break;
                        case CellState.Yang:
                            brush = Brushes.White;
                            break;
                        default:
                            brush = Brushes.LightGray;
                            break;
                    }

                    g.FillRectangle(brush, i * CellSize, j * CellSize, CellSize, CellSize);
                }
            }
        }

        private void MainForm_MouseClick(object sender, MouseEventArgs e)
        {
            int cellX = e.X / CellSize;
            int cellY = e.Y / CellSize;
            switch (grid[cellX, cellY])
            {
                case CellState.Dead:
                    grid[cellX, cellY] = CellState.Yin;
                    break;
                case CellState.Yin:
                    grid[cellX, cellY] = CellState.Yang;
                    break;
                case CellState.Yang:
                    grid[cellX, cellY] = CellState.Dead;
                    break;
                default:
                    grid[cellX, cellY] = CellState.Dead;
                    break;
            }

            Refresh();
        }

        private void EvolveGrid()
        {
            CellState[,] newGrid = new CellState[GridWidth, GridHeight];
            for (int i = 0; i < GridWidth; i++)
            {
                for (int j = 0; j < GridHeight; j++)
                {
                    int liveNeighbors = CountLiveNeighbors(i, j);

                    if (grid[i, j] == CellState.Dead && liveNeighbors == 3 && DifferentNeighborTypes(i, j))
                    {
                        // Рождення: випадкове створення клітини Інь або Ян
                        newGrid[i, j] = random.Next(0, 2) == 0 ? CellState.Yin : CellState.Yang;
                    }
                    else if (grid[i, j] != CellState.Dead && (liveNeighbors < 2 || liveNeighbors > 4))
                    {
                        // Гибель від перенаселення або гибель від одиночества: клітина помирає
                        newGrid[i, j] = CellState.Dead;
                    }
                    else if (grid[i, j] != CellState.Dead && !DifferentNeighborTypes(i, j))
                    {
                        // Гибель в неравном протистоянні: клітина помирає
                        newGrid[i, j] = CellState.Dead;
                    }
                    else
                    {
                        newGrid[i, j] = grid[i, j];
                    }
                }
            }
            grid = newGrid;
        }

        private int CountLiveNeighbors(int x, int y)
        {
            int count = 0;
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    if (i == 0 && j == 0)
                        continue;

                    int neighborX = x + i;
                    int neighborY = y + j;

                    if (neighborX >= 0 && neighborX < GridWidth && neighborY >= 0 && neighborY < GridHeight && grid[neighborX, neighborY] != CellState.Dead)
                        count++;
                }
            }
            return count;
        }

        private bool DifferentNeighborTypes(int x, int y)
        {
            bool hasYinNeighbor = false;
            bool hasYangNeighbor = false;

            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    if (i == 0 && j == 0)
                        continue;

                    int neighborX = x + i;
                    int neighborY = y + j;

                    if (neighborX >= 0 && neighborX < GridWidth && neighborY >= 0 && neighborY < GridHeight)
                    {
                        if (grid[neighborX, neighborY] == CellState.Yin)
                        {
                            if (hasYinNeighbor && hasYangNeighbor)
                                return true;

                            if (!hasYinNeighbor)
                                hasYinNeighbor = true;

                            if (!hasYangNeighbor)
                                hasYangNeighbor = true;
                        }
                        else if (grid[neighborX, neighborY] == CellState.Yang)
                        {
                            if (hasYinNeighbor && hasYangNeighbor)
                                return true;

                            if (!hasYangNeighbor)
                                hasYangNeighbor = true;

                            if (!hasYinNeighbor)
                                hasYinNeighbor = true;
                        }
                    }
                }
            }

            return false;
        }

        private void startButton_Click(object sender, EventArgs e)
        {
            timer.Start();
        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            timer.Stop();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            EvolveGrid();
            Refresh();
        }
    }

    public enum CellState
    {
        Dead,
        Yin,
        Yang
    }
}

