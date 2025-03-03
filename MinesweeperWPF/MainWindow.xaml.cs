using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Security.Policy;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using MinesweeperWPF.Model;
using static System.Reflection.Metadata.BlobBuilder;
using Label = System.Windows.Controls.Label;
using Point = System.Windows.Point;

namespace MinesweeperWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        MineSwipperModel model;

        public MainWindow()
        {
            InitializeComponent();
            model = new MineSwipperModel(this);
            this.DataContext = model;
            model.NewGame();
        }

        public void setWindowSize()
        {
            if(model.gridSize > 25)
            {
                this.Width = 900;
                this.Height = 900;
            }
            else if (model.gridSize > 19)
            {
                this.Width = 580;
                this.Height = 640;
            }
            else if (model.gridSize > 15)
            {
                this.Width = 510;
                this.Height = 570;
            }
            else if (model.gridSize > 11)
            {
                this.Width = 47 * model.gridSize;
                this.Height = 53 * model.gridSize;
            }
            else if (model.gridSize > 7)
            {
                this.Width = 430;
                this.Height = 490;
            }
            else
            {
                this.Width = 200;
                this.Height = 200;
            }
        }

        private void AdjustFontSize(object sender, SizeChangedEventArgs e)
        {
            TextBlock textBlock = sender as TextBlock;
            if (textBlock != null)
            {
                double newSize = Math.Min(e.NewSize.Width, e.NewSize.Height) * 0.6; 
                textBlock.FontSize = Math.Max(newSize, 10); 

                textBlock.LineHeight = textBlock.FontSize * 1.2;
            }
        }

        private void Button_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Button clickedButton = sender as Button;
            if (clickedButton == null) return;
            double width = clickedButton.ActualWidth;
            double height = clickedButton.ActualHeight;
            double size = (width + height) / 2;

            System.Windows.Point position = clickedButton.TransformToAncestor(this).Transform(new System.Windows.Point(0, 0));

            double x = position.X;
            double y = position.Y;
            GridCell cell = clickedButton.DataContext as GridCell;
            if (cell == null) return;
            else if (e.ChangedButton == MouseButton.Right)
            {
                model.ToogleFlag(cell, x, y, size);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button clickedButton = sender as Button;
            GridCell cell = clickedButton.DataContext as GridCell;
            double width = clickedButton.ActualWidth;
            double height = clickedButton.ActualHeight;
            double size = (width + height) / 2;

            System.Windows.Point position = clickedButton.TransformToAncestor(this).Transform(new System.Windows.Point(0, 0));

            double x = position.X;
            double y = position.Y;
            if (cell.hasFlag) return;
            model.verifieCellule(cell, x, y, size);
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Grid clickedCell = sender as Grid;
            if (clickedCell == null) return;
            double width = clickedCell.ActualWidth;
            double height = clickedCell.ActualHeight;
            double size = (width + height) / 2;

            System.Windows.Point position = clickedCell.TransformToAncestor(this).Transform(new System.Windows.Point(0, 0));

            double x = position.X;
            double y = position.Y;

            GridCell cell = clickedCell.DataContext as GridCell;
            if (cell == null || cell.visible) return;
            if (e.ChangedButton == MouseButton.Middle)
            {
                model.verifNeighbor(cell, x, y, size);
            }
        }

        private void TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
           this.Close();
        }

        public void Shake(double amplitude = 5, int durationMs = 500)
        {
            if (GRDGame == null) return;

            TranslateTransform transform = new TranslateTransform();
            GRDGame.RenderTransform = transform;

            Random rand = new Random();
            DoubleAnimationUsingKeyFrames animation = new DoubleAnimationUsingKeyFrames
            {
                Duration = TimeSpan.FromMilliseconds(durationMs)
            };

            for (int i = 0; i < 10; i++)
            {
                double offsetX = (rand.NextDouble() * 2 - 1) * amplitude;
                double offsetY = (rand.NextDouble() * 2 - 1) * amplitude;
                animation.KeyFrames.Add(new DiscreteDoubleKeyFrame(offsetX, KeyTime.FromPercent(i / 10.0)));
                animation.KeyFrames.Add(new DiscreteDoubleKeyFrame(offsetY, KeyTime.FromPercent(i / 10.0)));
            }

            animation.KeyFrames.Add(new DiscreteDoubleKeyFrame(0, KeyTime.FromPercent(1)));

            transform.BeginAnimation(TranslateTransform.XProperty, animation);
            transform.BeginAnimation(TranslateTransform.YProperty, animation);
        }

        private void GRDMain_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }
    }
}
