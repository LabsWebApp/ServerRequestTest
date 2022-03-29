using System.Collections;
using System.Text;

namespace ServerRequestTestLibrary.SingleRequests.Helper;

/// <summary>
/// Односвязный список
/// </summary>
/// <param name="Text">Значение</param>
/// <param name="InnerNode">Ссылка-связь</param>
public record Node(string Text, Node? InnerNode);

/// <summary>
/// Получение инфо об исключениях
/// </summary>
public class GetErrorMessage
{
    /// <summary>
    /// Входящий список связанных исключений
    /// </summary>
    public Node ErrorsTree { get; }

    private static string GetTabs(int tabs)
    {
        if (tabs < 1) return string.Empty;
        var sb = new StringBuilder(tabs);
        for (var i = 0; i < tabs; i++) sb.Append('\t');
        return sb.ToString();
    }

    /// <summary>
    /// Инициализация списка исключений
    /// </summary>
    /// <param name="exception">исключение</param>
    public GetErrorMessage(Exception exception)
    {
        string GetMsg(Exception exp) => exp.HResult switch
        {
            0 => exp.Message,
            _ => $"({exp.HResult}:) {exp.Message}"
        };

        string GetData(Exception exp, int tabs)
        {
            if (exp.Data.Count < 1) return string.Empty;

            var oldTabs = GetTabs(tabs);
            StringBuilder sb = new("\n");
            sb.Append(oldTabs);
            sb.Append("\tExtra data:");
            foreach (DictionaryEntry data in exp.Data)
            {
                sb.Append("\n\t\t");
                sb.Append(oldTabs);
                sb.Append('[');
                sb.Append(data.Key);
                sb.Append("] ");
                sb.Append(data.Value);
            }

            return sb.ToString();
        }

        Node? SetNode(Exception? exp, int tabs = 0) => exp switch
        {
            null => null,
            _ => new Node(
                $"{(tabs == 0 ? "":'\n')}{GetTabs(tabs)}{GetMsg(exp)}{GetData(exp, tabs)}",
                SetNode(exp.InnerException, ++tabs))
        };

        ErrorsTree = SetNode(exception)!;
    }

    public override string ToString()
    {
        StringBuilder sb = new(ErrorsTree.Text);
        var node = ErrorsTree;
        while (node.InnerNode is not null)
        {
            node = node.InnerNode;
            sb.Append(node.Text);
        }
        return sb.ToString();
    }

    /// <summary>
    /// Рекурсивно получает информацию об исключении и его внутреннего исключения
    /// </summary>
    /// <param name="exceptions">текущее исключение</param>
    /// <returns></returns>
    public static string GetExceptionsListInfo(IList<Exception>? exceptions) =>
        exceptions?.Any() switch
        {
            null or false => string.Empty,
            _ => exceptions
                .GroupBy(ex => ex, new ExceptionComparer())
                .Select(e => 
                    (value: new GetErrorMessage(e.Key).ToString(), count: e.Count()))
                .Aggregate(string.Empty, (x, y) =>
                {
                    var result = new StringBuilder();
                    if (!string.IsNullOrEmpty(x))
                    {
                        result.Append(x);
                        result.Append("\n\n");
                    }
                    result.Append(y.value);
                    if (y.count > 1) result.Append($"\n[entries count: {y.count}]");
                    return result.ToString();
                })
        };

    /// <summary>
    /// Статическое получение инфо об исключении
    /// </summary>
    /// <param name="exception">изучаемое исключение</param>
    /// <returns>строка с инфо об исключении и всех связанных с ним исключениях</returns>
    public static string GetExceptionInfo(Exception exception) =>
        new GetErrorMessage(exception).ToString();
}