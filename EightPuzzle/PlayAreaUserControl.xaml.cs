﻿using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using static EightPuzzle.Constants;

namespace EightPuzzle
{
    /// <summary>
    /// Interaction logic for PlayArea.xaml
    /// </summary>
    public partial class PlayAreaUserControl : UserControl, ISaveable
    {
        Puzzle puzzle = null;
        bool isStarted = false;

        public List<int> Positions => puzzle.ListOfPosition();

        /// <summary>
        /// PlayAreaUserControl constructor.
        /// </summary>
        /// <param name="source">Path of image.</param>
        public PlayAreaUserControl(string source)
        {
            InitializeComponent();
            puzzle = new Puzzle(source);
            while (!isSolvable())
                puzzle = new Puzzle(source);
            Initialize();
        }

        /// <summary>
        /// PlayAreaUserControl constructor.
        /// </summary>
        /// <param name="source">Path of image.</param>
        /// <param name="Pos_list">List of position.</param>
        public PlayAreaUserControl(BitmapImage image_source, List<int> Pos_list)
        {
            InitializeComponent();
            puzzle = new Puzzle(image_source, Pos_list);
            Initialize();
        }

        /// <summary>
        /// Initialize variables.
        /// </summary>
        private void Initialize()
        {
            isStarted = true;
            Container.Children.Clear();
            for (int i = 0; i < 3; ++i)
            {
                for (int j = 0; j < 3; ++j)
                {
                    if (puzzle.Images[i, j] != null)
                    {
                        Container.Children.Add(puzzle.Images[i, j]);

                        Canvas.SetLeft(puzzle.Images[i, j], i * (PUZZLE_SIZE.WIDTH + PUZZLE_PADDING));
                        Canvas.SetTop(puzzle.Images[i, j], j * (PUZZLE_SIZE.HEIGHT + PUZZLE_PADDING));
                    }
                }
            }
        }

        /// <summary>
        /// Check is enalbe to solve 8-puzzles.
        /// </summary>
        /// <returns></returns>
        public bool isSolvable()
        {
            var list = puzzle.ListOfPosition();
            int inv_count = 0;
            for (int i = 0; i < 9 - 1; i++)
                for (int j = i + 1; j < 9; j++)
                    if (list[i] < 8 && list[j] < 8 && list[i] > list[j])
                        inv_count++;
            return inv_count % 2 == 0;
        }

        public delegate void OnVictoryHandler();

        public event OnVictoryHandler OnVictory;
        void CheckVictory()
        {
            if (puzzle.isVictory())
                OnVictory?.Invoke();
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
            if (isStarted != true) return;
            var position = e.GetPosition(Container);
            var i = (int)(position.X / (PUZZLE_SIZE.WIDTH + PUZZLE_PADDING));
            var j = (int)(position.Y / (PUZZLE_SIZE.HEIGHT + PUZZLE_PADDING));

            if (i < 3 && j < 3)
            {
                Swap_Puzzle(i, j);
                CheckVictory();
            }
        }

        /// <summary>
        /// Handle while pressing a key.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (isStarted != true) return;
            Image empty_puzzle = new Image();
            int pos_x = -1;
            int pos_y = -1;
            for (int i = 0; i < 3; ++i)
            {
                for (int j = 0; j < 3; ++j)
                {
                    if (puzzle.Images[i, j] == null)
                    {
                        empty_puzzle = puzzle.Images[i, j];
                        pos_x = i;
                        pos_y = j;
                        break;
                    }
                }
            }

            try
            {
                if (e.Key == Key.Down || e.Key == Key.NumPad2)
                {
                    Debug.WriteLine(e.Key);
                    Swap_Puzzle(pos_x, pos_y - 1);
                    CheckVictory();
                    return;
                }
            }
            catch (Exception ex) { };
            try
            {
                if (e.Key == Key.Up || e.Key == Key.NumPad8)
                {
                    Debug.WriteLine(e.Key);
                    Swap_Puzzle(pos_x, pos_y + 1);
                    CheckVictory();
                    return;
                }
            }
            catch (Exception ex) { };
            try
            {
                if (e.Key == Key.Right || e.Key == Key.NumPad6)
                {
                    Debug.WriteLine(e.Key);
                    Swap_Puzzle(pos_x - 1, pos_y);
                    CheckVictory();
                    return;
                }
            }
            catch (Exception ex) { };
            try
            {
                if (e.Key == Key.Left || e.Key == Key.NumPad4)
                {
                    Debug.WriteLine(e.Key);
                    Swap_Puzzle(pos_x + 1, pos_y);
                    CheckVictory();
                    return;
                }
            }
            catch (Exception ex) { };
        }
    }
}