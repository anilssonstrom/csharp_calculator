using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Data;

namespace CSharp_Calc;

public partial class MainPageViewModel : ObservableObject
{
    // Set up all the private vars as properties
    [ObservableProperty]
    private string _expressionDisplay = string.Empty;

    [ObservableProperty]
    private string? _resultDisplay = string.Empty;

    [ObservableProperty]
    private int _cursorPosition = 0;

    // HandleButtonPress is called when a button is pushed
    [RelayCommand]
    public void HandleButtonPress(string buttonText)
    {
        // There are lots of bugs in here, but it's an initial stab at least.

        var curPos = CursorPosition;

        if (buttonText == "(  )")
        {
            // This needs to be properly tested, may be some gremlins.
            buttonText = ExpressionDisplay.ToCharArray().Where(x => x == '(' || x == ')')
                .Count() % 2 == 0 ? "(" : ")";
        }

        if (buttonText == "AC")
        {
            // Clear both display texts
            ExpressionDisplay = string.Empty;
            ResultDisplay = string.Empty;
        }
        else if (int.TryParse(buttonText, out var _) || buttonText == "%" || buttonText == ".")
        {
            // Update the expression with latest button push, while user is typing numbers
            var ch = buttonText[0];
            ExpressionDisplay = ExpressionDisplay.Insert(CursorPosition, ch.ToString());
            CursorPosition = curPos + 1;

            if (!double.TryParse(ExpressionDisplay, out var _))
            {
                try
                {
                    // Compute the result
                    double result = Convert.ToDouble(new DataTable().Compute(GenerateExpression(), null));
                    ResultDisplay = result.ToString();
                }
                catch
                {
                    // Do nothing
                }
            }
        }
        else if (buttonText == "=")
        {
            try
            {
                // Compute the result
                double result = Convert.ToDouble(new DataTable().Compute(GenerateExpression(), null));
                ResultDisplay = result.ToString();
            }
            catch
            {
                ResultDisplay = "Format error";
                return;
            }

            // Move result to expression field
            ExpressionDisplay = ResultDisplay;
            ResultDisplay = string.Empty;
        }
        else if (buttonText == "DEL")
        {
            // Delete latest input
            if (!string.IsNullOrEmpty(ExpressionDisplay))
                ExpressionDisplay = ExpressionDisplay.Remove(ExpressionDisplay.Length - 1);
        }
        else
        {
            // Just add the input and act like it's raining...
            var ch = buttonText[0];
            ExpressionDisplay = ExpressionDisplay.Insert(CursorPosition, ch.ToString());
            CursorPosition = curPos + 1;
        }
    }

    // Replace certain display chars with chars the calculator can handle. Some simple assumptions here that don't really hold up. Handling of parentheses need to be improved.
    private string GenerateExpression() => ExpressionDisplay.Replace('×', '*').Replace('÷', '/').Replace("%", "*0.01").Replace('(', '*').Replace(")*", "*").Replace(")", "*");
}
