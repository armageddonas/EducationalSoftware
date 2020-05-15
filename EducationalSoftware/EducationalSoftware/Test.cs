﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EducationalSoftware
{
    public partial class TestForm : Form
    {
        public TestForm()
        {
            InitializeComponent();
            
        }

        private int TotalQuestionsNum;
        private int ThisQuestionNum = 0;
        private int points = 0;
        Random rnd = new Random();

        private int[] done = new int[10];

        PictureBox[] boxes;
        NumKeyboard keys;
        private void TestForm_Load(object sender, EventArgs e)
        {
            DifficultyGroup.Location = new Point(this.Size.Width / 2 - DifficultyGroup.Size.Width / 2, this.Size.Height / 2 - DifficultyGroup.Size.Height / 2);
            keys = new NumKeyboard(picture_r1, picture_r2, picture_res1, picture_res2, picture_res3, RightNum, ResultNum);
        }

        private void EasyButton_Click(object sender, EventArgs e)
        {
            TotalQuestionsNum = 10;
            CreateQuestionaire();
        }        

        private void MediumButton_Click(object sender, EventArgs e)
        {
            TotalQuestionsNum = 15;
            CreateQuestionaire();
        }

        private void HardButton_Click(object sender, EventArgs e)
        {
            TotalQuestionsNum = 20;
            CreateQuestionaire();
        }

        /// <summary>
        /// Is called when difficulty is chosen. Makes Questionaire visible.
        /// </summary>
        private void CreateQuestionaire()
        {
            DifficultyGroup.Visible = false;
            QuestionGroup.Location = new Point(this.Size.Width / 2 - QuestionGroup.Size.Width / 2, this.Size.Height / 2 - QuestionGroup.Size.Height / 2);
            QuestionGroup.Visible = true;
            QuestionLabel.Visible = true;
            ThisQuestionsText.Visible = true;
            QuestionSlash.Visible = true;
            TotalQuestionsText.Visible = true;
            TotalQuestionsText.Text = TotalQuestionsNum.ToString();
            CreateQuestion();
        }
        /// <summary>
        /// chooses randomly where the blank number will be and creates the equation.
        /// </summary>
        private void CreateQuestion()
        {
            picture_l1.Image = null;
            picture_l2.Image = null;
            picture_r1.Image = null;
            picture_r2.Image = null;
            picture_res1.Image = null;
            picture_res2.Image = null;
            picture_res3.Image = null;

            ThisQuestionNum++;
            ThisQuestionsText.Text = ThisQuestionNum.ToString();
            
            int rightnum = rnd.Next(0, 11);
            int leftnum;
            do
            {
                leftnum = rnd.Next(1, 11);
            } while (done[leftnum-1]==1);
            done[leftnum - 1] = 1;
            if (!done.Contains(0))
            {
                Array.Clear(done, 0, done.Length);
            }
            Equation eq = new Equation(leftnum, rightnum, "right");

            int blank = rnd.Next(0, 2);//chooses randomly which number box of the multiplication will be blank.

            
            if (eq.left_mult_digits.Count == 2)
            {
                picture_l1.Image = (Image)Properties.Resources.ResourceManager.GetObject("num_" + eq.left_mult_digits[1]);
                picture_l2.Image = (Image)Properties.Resources.ResourceManager.GetObject("num_" + eq.left_mult_digits[0]);
            }
            else
            {
                picture_l1.Image = (Image)Properties.Resources.ResourceManager.GetObject("num_" + eq.left_mult_digits[0]);
            }


            LeftNum.Value = leftnum;
            if (blank == 0)//the right number box is blank.
            {
                ResultNum.Value = leftnum*rightnum;
                RightNum.Value = 0;        

                if (eq.equal_digits.Count >= 2)
                {
                    if (eq.right_mult_digits.Count == 3)
                    {
                        picture_res1.Image = (Image)Properties.Resources.ResourceManager.GetObject("num_" + eq.equal_digits[2]);
                        picture_res2.Image = (Image)Properties.Resources.ResourceManager.GetObject("num_" + eq.equal_digits[1]);
                        picture_res3.Image = (Image)Properties.Resources.ResourceManager.GetObject("num_" + eq.equal_digits[0]);
                    }
                    else
                    {
                        picture_res1.Image = (Image)Properties.Resources.ResourceManager.GetObject("num_" + eq.equal_digits[1]);
                        picture_res2.Image = (Image)Properties.Resources.ResourceManager.GetObject("num_" + eq.equal_digits[0]);
                    }
                }
                else
                {
                    picture_res1.Image = (Image)Properties.Resources.ResourceManager.GetObject("num_" + eq.equal_digits[0]);
                }

                

                picture_r1.Image = Properties.Resources.questionmark;
                

                picture_r1.Tag = "empty";
                picture_r2.Tag = "empty";

                picture_res1.Tag = "given";
                picture_res2.Tag = "given";
                picture_res3.Tag = "given";
            }
            else//the result box is blank.
            {
                ResultNum.Value = 0;
                RightNum.Value = rightnum;
                                
                if (eq.right_mult_digits.Count == 2)
                {
                    picture_r1.Image = (Image)Properties.Resources.ResourceManager.GetObject("num_" + eq.right_mult_digits[1]);
                    picture_r2.Image = (Image)Properties.Resources.ResourceManager.GetObject("num_" + eq.right_mult_digits[0]);
                }
                else
                {
                    picture_r1.Image = (Image)Properties.Resources.ResourceManager.GetObject("num_" + eq.right_mult_digits[0]);
                }

                picture_res1.Image = Properties.Resources.questionmark;
                
                picture_res1.Tag = "empty";
                picture_res2.Tag = "empty";
                picture_res3.Tag = "empty";

                picture_r1.Tag = "given";
                picture_r2.Tag = "given";
            }
        }

        private void ConfirmButton_Click(object sender, EventArgs e)
        {
            keys.FixResult();
            if (LeftNum.Value * RightNum.Value == ResultNum.Value)
            {
                points++;
                MessageBox.Show("Correct Answer! Well Done");
            }
            else
            {
                MessageBox.Show("Oh, no! Wrong answer. \nBetter luck next time!");
            }
            if (ThisQuestionNum < TotalQuestionsNum)
            {
                ConfirmButton.Enabled = true;
                BackButton.Enabled = false;
                CreateQuestion();
            }
            else
            {
                MessageBox.Show("yay! "+points.ToString()+" out of "+TotalQuestionsNum.ToString()+" correct answets!");//test
                BackButton.Enabled = true;
                ConfirmButton.Enabled = false;
            }            
        }


        private void AddNumber(object sender, EventArgs e)
        {
            keys.CheckEmpty(sender);
        }


        private void del_button_Click(object sender, EventArgs e)
        {
            keys.DelNumber();
        }
    }
}
