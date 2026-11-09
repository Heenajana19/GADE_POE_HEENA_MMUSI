using System;
using System.Windows.Forms;

namespace GADE_POE_HEENA_JANA
{
    public partial class Form1 : Form
    {
        private GameEngine engine;

        public Form1()
        {
            InitializeComponent();

            // Initialize the 10x10 map grid
            engine = new GameEngine(10, 10);

            this.KeyPreview = true;
            this.KeyDown += Form1_KeyDown;

            UpdateUI();
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.W:
                case Keys.Up:
                    engine.MoveHero(Direction.Up);
                    break;

                case Keys.S:
                case Keys.Down:
                    engine.MoveHero(Direction.Down);
                    break;

                case Keys.A:
                case Keys.Left:
                    engine.MoveHero(Direction.Left);
                    break;

                case Keys.D:
                case Keys.Right:
                    engine.MoveHero(Direction.Right);
                    break;
            }

            UpdateUI();
        }

        private void UpdateUI()
        {
            // Display the map
            rtbMap.Clear();

            for (int y = 0; y < engine.CurrentLevel.Height; y++)
            {
                for (int x = 0; x < engine.CurrentLevel.Width; x++)
                {
                    Tile tile = engine.CurrentLevel.Grid[x, y];
                    rtbMap.AppendText(tile.ToString() + " ");
                }

                rtbMap.AppendText("\n");
            }

            // Display hero health
            if (lblStats != null)
            {
                lblStats.Text =
                    $"Hero HP: {engine.CurrentLevel.Hero.CurrentHealth} / " +
                    $"{engine.CurrentLevel.Hero.MaxHealth}";

                if (engine.CurrentLevel.Hero.IsDead)
                {
                    lblStats.Text += " | GAME OVER!";
                }
            }
        }
    }
}

using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace GADE_POE_HEENA_JANA
{
    public class Form1 : Form
    {
        private GameEngine engine;

        private Label lblDisplay;
        private Label lblStats;
        private Label lblHelp;

        public Form1()
        {
            Text = "Heena and Mmusi - Final POE";
            ClientSize = new Size(1000, 800);
            StartPosition = FormStartPosition.CenterScreen;
            AutoScroll = true;

            BuildInterface();

            engine = new GameEngine(10);
            UpdateDisplay();
        }

        private void BuildInterface()
        {
            lblStats = new Label
            {
                Location = new Point(20, 15),
                Size = new Size(950, 35),
                Font = new Font("Segoe UI", 11, FontStyle.Bold)
            };

            lblDisplay = new Label
            {
                Location = new Point(20, 60),
                Size = new Size(950, 540),
                AutoSize = false,
                Font = new Font("Consolas", 13),
                BackColor = Color.FromArgb(25, 25, 30),
                ForeColor = Color.White,
                Padding = new Padding(10),
                UseMnemonic = false
            };

            lblHelp = new Label
            {
                Location = new Point(20, 610),
                Size = new Size(950, 75),
                Font = new Font("Segoe UI", 10),
                Text =
                    "Move: WASD or arrow keys | Attack: I up, J left, K down, L right\r\n" +
                    "▼ Hero   Ϫ Grunt   ᐂ Warlock   § Tyrant   + Health   * Double damage\r\n" +
                    "▓ Locked exit   ▒ Open exit   x Defeated enemy"
            };

            Button saveButton = MakeButton("Save Game", 20);
            Button loadButton = MakeButton("Load Game", 180);
            Button restartButton = MakeButton("New Game", 340);

            saveButton.Click += delegate
            {
                try
                {
                    engine.SaveGame();

                    MessageBox.Show(
                        this,
                        "Game saved successfully.",
                        "Save Game");
                }
                catch (Exception exception)
                {
                    MessageBox.Show(
                        this,
                        "Could not save the game:\n" + exception.Message,
                        "Save Error");
                }
            };

            loadButton.Click += delegate
            {
                try
                {
                    engine.LoadGame();
                    UpdateDisplay();

                    MessageBox.Show(
                        this,
                        "Saved game loaded.",
                        "Load Game");
                }
                catch (Exception exception)
                {
                    MessageBox.Show(
                        this,
                        "Could not load the game:\n" + exception.Message,
                        "Load Error");
                }
            };

            restartButton.Click += delegate
            {
                DialogResult result = MessageBox.Show(
                    this,
                    "Start a new game? Unsaved progress will be lost.",
                    "New Game",
                    MessageBoxButtons.YesNo);

                if (result == DialogResult.Yes)
                {
                    engine = new GameEngine(10);
                    UpdateDisplay();
                }
            };

            Controls.AddRange(new Control[]
            {
                lblStats,
                lblDisplay,
                lblHelp,
                saveButton,
                loadButton,
                restartButton
            });
        }

        private Button MakeButton(string text, int x)
        {
            return new Button
            {
                Text = text,
                Location = new Point(x, 700),
                Size = new Size(145, 40),
                TabStop = false
            };
        }

        // Handles arrow keys even when a button has focus.
        protected override bool ProcessCmdKey(
            ref Message message,
            Keys keyData)
        {
            if (engine == null)
            {
                return base.ProcessCmdKey(ref message, keyData);
            }

            // Leave modified shortcuts to Windows Forms.
            if ((keyData & Keys.Modifiers) != Keys.None)
            {
                return base.ProcessCmdKey(ref message, keyData);
            }

            switch (keyData & Keys.KeyCode)
            {
                case Keys.W:
                case Keys.Up:
                    engine.TriggerMovement(Direction.Up);
                    break;

                case Keys.D:
                case Keys.Right:
                    engine.TriggerMovement(Direction.Right);
                    break;

                case Keys.S:
                case Keys.Down:
                    engine.TriggerMovement(Direction.Down);
                    break;

                case Keys.A:
                case Keys.Left:
                    engine.TriggerMovement(Direction.Left);
                    break;

                case Keys.I:
                    engine.TriggerAttack(Direction.Up);
                    break;

                case Keys.L:
                    engine.TriggerAttack(Direction.Right);
                    break;

                case Keys.K:
                    engine.TriggerAttack(Direction.Down);
                    break;

                case Keys.J:
                    engine.TriggerAttack(Direction.Left);
                    break;

                default:
                    return base.ProcessCmdKey(ref message, keyData);
            }

            UpdateDisplay();
            return true;
        }

        private void UpdateDisplay()
        {
            lblDisplay.Text = engine.ToString();

            int enemiesRemaining =
                engine.CurrentLevel.Enemies.Count(enemy => !enemy.IsDead);

            lblStats.Text =
                "Level " + engine.CurrentLevelNumber +
                "/" + engine.NumberOfLevels +
                " | HP: " + engine.HeroStats +
                " | Enemies: " + enemiesRemaining +
                " | Double-damage attacks: " +
                engine.CurrentLevel.Hero.DoubleDamageCount +
                " | " + engine.State;
        }
    }
}
