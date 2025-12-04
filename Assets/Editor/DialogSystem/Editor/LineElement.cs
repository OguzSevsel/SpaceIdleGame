using UnityEngine;
using UnityEngine.UIElements;

public class LineElement : VisualElement
{
    private readonly VisualElement lineFrom;
    private readonly VisualElement lineTo;

    public NodeElement NodeFrom { get; private set; }
    public NodeElement NodeTo { get; private set; }

    public TextField OptionText { get; private set; }
    public VisualElement Canvas { get; private set; }

    public LineElement(VisualElement from, VisualElement to, VisualElement canvas, NodeElement nodeFrom = null, NodeElement nodeTo = null)
    {
        this.lineFrom = from;
        this.lineTo = to;
        this.NodeFrom = nodeFrom;
        this.NodeTo = nodeTo;
        this.Canvas = canvas;

        pickingMode = PickingMode.Ignore;
        style.position = Position.Absolute;

        generateVisualContent += OnGenerateVisualContent;
    }

    public void UpdateTextboxPosition()
    {
        if (lineFrom == null || lineTo == null || this.parent == null || Canvas == null) return;

        Vector2 fromCenter = lineFrom.worldBound.center;
        Vector2 toCenter = lineTo.worldBound.center;
        Vector2 midWorld = (fromCenter + toCenter) / 2f;

        Vector2 midLocal = Canvas.contentContainer.WorldToLocal(midWorld);

        OptionText.style.left = midLocal.x - OptionText.resolvedStyle.width / 2f;
        OptionText.style.top = midLocal.y - OptionText.resolvedStyle.height / 2f;

        if (!Canvas.Contains(OptionText))
            Canvas.Add(OptionText);
    }


    public void CreateTextBox()
    {
        var textBox = new TextField();
        textBox.style.position = Position.Absolute;
        textBox.style.width = 100;
        textBox.style.unityTextAlign = TextAnchor.MiddleCenter;

        Vector2 fromCenter = lineFrom.worldBound.center;
        Vector2 toCenter = lineTo.worldBound.center;
        Vector2 midWorld = (fromCenter + toCenter) / 2f;

        Vector2 midLocal = Canvas.contentContainer.WorldToLocal(midWorld);

        textBox.style.left = midLocal.x - 50; 
        textBox.style.top = midLocal.y - 10;  

        // Optional styling
        textBox.style.borderBottomLeftRadius = 4;
        textBox.style.borderBottomRightRadius = 4;
        textBox.style.borderTopLeftRadius = 4;
        textBox.style.borderTopRightRadius = 4;
        textBox.multiline = true;

        OptionText = textBox;
        Canvas.Add(OptionText);
        Helpers.SetMarginsAndPadding(OptionText, 0);

        textBox.Focus();
    }

    private void OnGenerateVisualContent(MeshGenerationContext ctx)
    {
        if (lineFrom == null || lineTo == null) return;

        Rect fromRectWorld = lineFrom.worldBound;
        Rect toRectWorld = lineTo.worldBound;

        Vector2 fromCenterWorld = fromRectWorld.center;
        Vector2 toCenterWorld = toRectWorld.center;

        Vector2 worldDir = (toCenterWorld - fromCenterWorld);
        float dist = worldDir.magnitude;
        if (dist < 0.001f)
            return;

        Vector2 worldDirNorm = worldDir / dist;

        Vector2 startWorld = GetEdgePointWorld(fromRectWorld, worldDirNorm);
        Vector2 endWorld = GetEdgePointWorld(toRectWorld, -worldDirNorm);

        Vector2 startLocal = this.WorldToLocal(startWorld);
        Vector2 endLocal = this.WorldToLocal(endWorld);

        if (!IsFiniteVector(startLocal) || !IsFiniteVector(endLocal))
        {
            startLocal = this.WorldToLocal(fromCenterWorld);
            endLocal = this.WorldToLocal(toCenterWorld);
        }

        var painter = ctx.painter2D;
        painter.lineWidth = 3f;
        painter.strokeColor = Helpers.HexToColor(Helpers.ColorBorder);

        painter.BeginPath();
        painter.MoveTo(startLocal);
        painter.LineTo(endLocal);
        painter.Stroke();

        Vector2 dirLocal = (endLocal - startLocal).normalized;
        if (dirLocal.sqrMagnitude < 0.0001f) dirLocal = Vector2.up;

        Vector2 perp = new Vector2(-dirLocal.y, dirLocal.x);
        Vector2 arrowLeft = endLocal - dirLocal * 12f + perp * 6f;
        Vector2 arrowRight = endLocal - dirLocal * 12f - perp * 6f;

        painter.BeginPath();
        painter.MoveTo(endLocal);
        painter.LineTo(arrowLeft);
        painter.LineTo(arrowRight);
        painter.ClosePath();
        painter.fillColor = Helpers.HexToColor(Helpers.ColorBorder);
        painter.Fill();
    }

    private Vector2 GetEdgePointWorld(Rect rect, Vector2 dir)
    {
        Vector2 absDir = new Vector2(Mathf.Abs(dir.x), Mathf.Abs(dir.y));

        if (rect.width <= 0 || rect.height <= 0)
            return rect.center;

        if (absDir.x > absDir.y)
        {
            // horizontal edge
            if (dir.x > 0)
                return new Vector2(rect.xMax, rect.center.y);
            else
                return new Vector2(rect.xMin, rect.center.y);
        }
        else
        {
            // vertical edge
            if (dir.y > 0)
                return new Vector2(rect.center.x, rect.yMax);
            else
                return new Vector2(rect.center.x, rect.yMin);
        }
    }

    private bool IsFiniteVector(Vector2 v)
    {
        return float.IsFinite(v.x) && float.IsFinite(v.y) && Mathf.Abs(v.x) < 1e8f && Mathf.Abs(v.y) < 1e8f;
    }
}



