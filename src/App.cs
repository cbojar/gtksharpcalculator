using Gtk;

public class App
{
	public static void Main(string[] args)
	{
		Application.Init();

		UI app = new UI();

		app.Show();

		Application.Run();
	}
}

