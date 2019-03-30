using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Gtk;
using Pango;

public class UI
{
    private static readonly Regex VALID_COMPUTATION = new Regex(@"^\-?\d+(\.\d+)?( [\+\-\*/] \-?\d+(\.\d+)?)*$");
    private const uint BUTTONS_WIDE = 4;
	private const uint BUTTONS_TALL = 5;

    private readonly Window window;
	private readonly TextView input;
	private readonly TextView output;

    public UI() 
    {
		window = new Window("Calculator");
		window.DeleteEvent += new DeleteEventHandler(OnClose);
		window.Resize(300, 300);

		VBox layout = new VBox();
		layout.Homogeneous = false;

		input = new TextView();
		input.Editable = false;
		input.CursorVisible = false;
		input.Justification = Justification.Right;
		input.LeftMargin = 16;
		input.RightMargin = 16;

		layout.PackStart(input, true, true, 0);

		output = new TextView();
		output.Editable = false;
		output.CursorVisible = false;
		output.Justification = Justification.Right;
		output.LeftMargin = 16;
		output.RightMargin = 16;
        output.OverrideFont(FontDescription.FromString("Bold 16"));

		layout.PackStart(output, true, true, 0);

		Table table = new Table(BUTTONS_TALL, BUTTONS_WIDE, true);

		table.Attach(InputButton("9"), 2, 3, 0, 1);
		table.Attach(InputButton("8"), 1, 2, 0, 1);
		table.Attach(InputButton("7"), 0, 1, 0, 1);
		table.Attach(InputButton("6"), 2, 3, 1, 2);
		table.Attach(InputButton("5"), 1, 2, 1, 2);
		table.Attach(InputButton("4"), 0, 1, 1, 2);
		table.Attach(InputButton("3"), 2, 3, 2, 3);
		table.Attach(InputButton("2"), 1, 2, 2, 3);
		table.Attach(InputButton("1"), 0, 1, 2, 3);
		table.Attach(InputButton("0"), 0, 1, 3, 4);

		table.Attach(InputButton("."), 1, 2, 3, 4);
		table.Attach(InputButton("-"), 1, 2, 4, 5); // Negate

		table.Attach(OperationButton("+"), 3, 4, 0, 1);
		table.Attach(OperationButton("-"), 3, 4, 1, 2);
		table.Attach(OperationButton("*"), 3, 4, 2, 3);
		table.Attach(OperationButton("/"), 3, 4, 3, 4);
		table.Attach(CreateButton("=", OnOutputClick), 2, 3, 3, 4);
		table.Attach(CreateButton("C", OnClearClick), 3, 4, 4, 5);
		table.Attach(CreateButton("<<", OnBackspaceClick), 2, 3, 4, 5);

		layout.PackStart(table, true, true, 0);

		window.Add(layout);
	}

	private Button InputButton(string value) => CreateButton(value, OnInputClick);

	private Button OperationButton(string operation) => CreateButton(operation, OnOperationClick);

	private Button CreateButton(string value, Action<object, EventArgs> handler)
	{
		Button button = new Button(value);
		button.Clicked += new EventHandler(handler);

		return button;
	}

	public void Show()
	{
		// Fix resizing issue
		input.Buffer.Text = " ";
		output.Buffer.Text = " ";

		window.ShowAll();

		input.Buffer.Text = "";
		output.Buffer.Text = "";
	}

	private static void OnClose(object obj, DeleteEventArgs args)
	{
		Application.Quit();
	}

	private void OnInputClick(object obj, EventArgs args)
	{
		if (output.Buffer.Text != "")
		{
			input.Buffer.Text = "";
		}

		input.Buffer.Text += ((Button)obj).Label;
		output.Buffer.Text = "";
	}

	private void OnOperationClick(object obj, EventArgs args)
	{
		string currentOutput = output.Buffer.Text;

		if (IsNumber(currentOutput))
		{
			input.Buffer.Text = currentOutput;
		}

		input.Buffer.Text += " " + ((Button)obj).Label + " ";
		output.Buffer.Text = "";
	}

	private static bool IsNumber(string str)
	{
        decimal unused = 0;
        return decimal.TryParse(str, out unused);
	}

	private void OnOutputClick(object obj, EventArgs args)
	{
		if (!VALID_COMPUTATION.IsMatch(input.Buffer.Text))
		{
			output.Buffer.Text = "ERROR";
			return;
		}

		try
		{
			decimal answer = Calculator.calculate(Tokenizer.Tokenize(input.Buffer.Text));

			output.Buffer.Text = answer.ToString();
		}
		catch (DivideByZeroException ex)
		{
			Console.WriteLine(ex.ToString());
			output.Buffer.Text = "DIV#0";
		}
		catch (OverflowException ex)
		{
			Console.WriteLine(ex.ToString());
			output.Buffer.Text = "OVERFLOW";	
		}
	}

	private void OnClearClick(object obj, EventArgs args)
	{
		input.Buffer.Text = "";
		output.Buffer.Text = "";
	}

	private void OnBackspaceClick(object obj, EventArgs args)
	{
		output.Buffer.Text = "";

		string currentInput = input.Buffer.Text;

		if (currentInput == "")
		{
			return;
		}

		if (currentInput[currentInput.Length - 1] == ' ') // operator
		{
			input.Buffer.Text = currentInput.Substring(0, currentInput.Length - 3);
		}
		else
		{
			input.Buffer.Text = currentInput.Substring(0, currentInput.Length - 1);
		}
	}
}