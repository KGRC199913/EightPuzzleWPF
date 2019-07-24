using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static EightPuzzle.Constrants;

namespace EightPuzzle
{
    /// <summary>
    /// Interaction logic for PlayArea.xaml
    /// </summary>
    public partial class PlayAreaUserControl : UserControl
    {
        public PlayAreaUserControl()
        {
            InitializeComponent();
        }

        Puzzle puzzle = null;

        /// <summary>
        /// Insert an image to play.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var scr = new OpenFileDialog();
            if (scr.ShowDialog() == true)
            {
                puzzle = new Puzzle(scr.FileName);

                for (int i = 0; i < 3; ++i)
                {
                    for (int j = 0; j < 3; ++j)
                    {
                        if (i != 2 || j != 2)
                        {
                            Container.Children.Add(puzzle.Images[i, j]);

                            Canvas.SetLeft(puzzle.Images[i, j], i * (PUZZLE_SIZE.WIDTH + PUZZLE_PADDING));
                            Canvas.SetTop(puzzle.Images[i, j], j * (PUZZLE_SIZE.HEIGHT + PUZZLE_PADDING));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Swap the selected puzzle with the empty (sub-function).
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        private void Swap_Puzzle(int i, int j)
        {
            //Because of some puzzle are out of range so try/catch is used to handle them.
            if (puzzle.Images[i, j] != null)
            {
                try
                {
                    if (puzzle.Images[i - 1, j] == null)
                    {
                        Canvas.SetLeft(puzzle.Images[i, j], (i - 1) * (PUZZLE_SIZE.WIDTH + PUZZLE_PADDING));
                        Canvas.SetTop(puzzle.Images[i, j], j * (PUZZLE_SIZE.HEIGHT + PUZZLE_PADDING));
                        var tmp = puzzle.Images[i, j];
                        puzzle.Images[i, j] = puzzle.Images[i - 1, j];
                        puzzle.Images[i - 1, j] = tmp;
                        return;
                    }
                }
                catch (Exception ex) { };
                try
                {
                    if (puzzle.Images[i + 1, j] == null)
                    {
                        Canvas.SetLeft(puzzle.Images[i, j], (i + 1) * (PUZZLE_SIZE.WIDTH + PUZZLE_PADDING));
                        Canvas.SetTop(puzzle.Images[i, j], j * (PUZZLE_SIZE.HEIGHT + PUZZLE_PADDING));
                        var tmp = puzzle.Images[i, j];
                        puzzle.Images[i, j] = puzzle.Images[i + 1, j];
                        puzzle.Images[i + 1, j] = tmp;
                        return;
                    }
                }
                catch (Exception ex) { };
                try
                {
                    if (puzzle.Images[i, j - 1] == null)
                    {
                        Canvas.SetLeft(puzzle.Images[i, j], i * (PUZZLE_SIZE.WIDTH + PUZZLE_PADDING));
                        Canvas.SetTop(puzzle.Images[i, j], (j - 1) * (PUZZLE_SIZE.HEIGHT + PUZZLE_PADDING));
                        var tmp = puzzle.Images[i, j];
                        puzzle.Images[i, j] = puzzle.Images[i, j - 1];
                        puzzle.Images[i, j - 1] = tmp;
                        return;
                    }
                }
                catch (Exception ex) { };
                try
                {
                    if (puzzle.Images[i, j + 1] == null)
                    {
                        Canvas.SetLeft(puzzle.Images[i, j], i * (PUZZLE_SIZE.WIDTH + PUZZLE_PADDING));
                        Canvas.SetTop(puzzle.Images[i, j], (j + 1) * (PUZZLE_SIZE.HEIGHT + PUZZLE_PADDING));
                        var tmp = puzzle.Images[i, j];
                        puzzle.Images[i, j] = puzzle.Images[i, j + 1];
                        puzzle.Images[i, j + 1] = tmp;
                        return;
                    }
                }
                catch (Exception ex) { };
            }
        }

        /// <summary>
        /// Swap the selected puzzle with the empty.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Container_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var position = e.GetPosition(Container);
            var i = (int)(position.X / (PUZZLE_SIZE.WIDTH + PUZZLE_PADDING));
            var j = (int)(position.Y / (PUZZLE_SIZE.HEIGHT + PUZZLE_PADDING));

            Swap_Puzzle(i, j);
        }
    }
}