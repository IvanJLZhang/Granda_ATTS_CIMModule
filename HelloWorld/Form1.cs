using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Granda.ATTS.CIMModule.Scenario;
namespace HelloWorld
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            //Stack<List<Item>> stack = new Stack<List<Item>>();
            //stack.Push(new List<Item>());
            //var item = A("0");
            //stack.Peek().Add(item);
            //stack.Push(new List<Item>());
            //stack.Peek().Add(A("MDLN"));
            //stack.Peek().Add(A("SOFTREV"));
            //item = ParseItem(stack);
            InitializeScenario initializeScenario = new InitializeScenario();
            initializeScenario.CustomMethod();
        }
    }
}
