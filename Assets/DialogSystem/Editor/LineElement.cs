using UnityEngine;
using UnityEngine.UIElements;

public class LineElement : VisualElement
{
    private readonly VisualElement lineFrom;
    private readonly VisualElement lineTo;

    public NodeElement NodeFrom { get; private set; }
    public NodeElement NodeTo { get; private set; }

    public TextField OptionText { get; private set; }
    public ScrollView ScrollView { get; private set; }

    public LineElement(VisualElement from, VisualElement to, ScrollView scrollView, NodeElement nodeFrom = null, NodeElement nodeTo = null)
    {
        this.lineFrom = from;
        this.lineTo = to;
        this.NodeFrom = nodeFrom;
        this.NodeTo = nodeTo;
        this.ScrollView = scrollView;

        pickingMode = PickingMode.Ignore;
        style.position = Position.Absolute;

        generateVisualContent += OnGenerateVisualContent;
    }

    public void UpdateTextboxPosition()
    {
        if (lineFrom == null || lineTo == null || this.parent == null || ScrollView == null) return;

        // Convert world midpoint to the same space as the textbox parent
        Vector2 fromCenter = lineFrom.worldBound.center;
        Vector2 toCenter = lineTo.worldBound.center;
        Vector2 midWorld = (fromCenter + toCenter) / 2f;

        // Convert world position to local relative to the scrolling content
        Vector2 midLocal = ScrollView.contentContainer.WorldToLocal(midWorld);

        // Update textbox position
        OptionText.style.left = midLocal.x - OptionText.resolvedStyle.width / 2f;
        OptionText.style.top = midLocal.y - OptionText.resolvedStyle.height / 2f;

        // Ensure textbox is under the same scrolling content
        if (!ScrollView.contentContainer.Contains(OptionText))
            ScrollView.contentContainer.Add(OptionText);
    }


    public void CreateTextBox()
    {
        // Create textbox
        var textBox = new TextField();
        textBox.style.position = Position.Absolute;
        textBox.style.width = 100;
        textBox.style.height = 20;
        textBox.style.unityTextAlign = TextAnchor.MiddleCenter;

        // Compute world-space midpoint between centers
        Vector2 fromCenter = lineFrom.worldBound.center;
        Vector2 toCenter = lineTo.worldBound.center;
        Vector2 midWorld = (fromCenter + toCenter) / 2f;

        // Convert world position to parent local (so it's positioned correctly)
        Vector2 midLocal = ScrollView.contentContainer.WorldToLocal(midWorld);

        // Position textbox so it’s centered
        textBox.style.left = midLocal.x - 50; // half of width
        textBox.style.top = midLocal.y - 10;  // small offset

        // Optional styling
        textBox.style.backgroundColor = new Color(0.2f, 0.2f, 0.2f);
        textBox.style.color = Color.white;
        textBox.style.borderBottomLeftRadius = 4;
        textBox.style.borderBottomRightRadius = 4;
        textBox.style.borderTopLeftRadius = 4;
        textBox.style.borderTopRightRadius = 4;
        textBox.multiline = true;

        // Add to the same parent (so it appears above the line)
        OptionText = textBox;
        ScrollView.contentContainer.Add(OptionText);

        // Focus immediately so user can type
        textBox.Focus();
    }

    private void OnGenerateVisualContent(MeshGenerationContext ctx)
    {
        if (lineFrom == null || lineTo == null) return;

        // World-space rects
        Rect fromRectWorld = lineFrom.worldBound;
        Rect toRectWorld = lineTo.worldBound;

        // World-space centers
        Vector2 fromCenterWorld = fromRectWorld.center;
        Vector2 toCenterWorld = toRectWorld.center;

        // Direction from A -> B in world space
        Vector2 worldDir = (toCenterWorld - fromCenterWorld);
        float dist = worldDir.magnitude;
        if (dist < 0.001f)
            return;

        Vector2 worldDirNorm = worldDir / dist;

        // compute best edge points in world space
        Vector2 startWorld = GetEdgePointWorld(fromRectWorld, worldDirNorm);
        Vector2 endWorld = GetEdgePointWorld(toRectWorld, -worldDirNorm);

        // convert world-space points into this element's local space
        Vector2 startLocal = this.WorldToLocal(startWorld);
        Vector2 endLocal = this.WorldToLocal(endWorld);

        // if conversion produced NaNs or huge values (defensive), fallback to centers
        if (!IsFiniteVector(startLocal) || !IsFiniteVector(endLocal))
        {
            startLocal = this.WorldToLocal(fromCenterWorld);
            endLocal = this.WorldToLocal(toCenterWorld);
        }

        var painter = ctx.painter2D;
        painter.lineWidth = 3f;
        painter.strokeColor = Color.white;

        // Draw main line
        painter.BeginPath();
        painter.MoveTo(startLocal);
        painter.LineTo(endLocal);
        painter.Stroke();

        // Draw arrowhead at 'endLocal'
        Vector2 dirLocal = (endLocal - startLocal).normalized;
        if (dirLocal.sqrMagnitude < 0.0001f) dirLocal = Vector2.up; // fallback

        Vector2 perp = new Vector2(-dirLocal.y, dirLocal.x);
        Vector2 arrowLeft = endLocal - dirLocal * 12f + perp * 6f;
        Vector2 arrowRight = endLocal - dirLocal * 12f - perp * 6f;

        painter.BeginPath();
        painter.MoveTo(endLocal);
        painter.LineTo(arrowLeft);
        painter.LineTo(arrowRight);
        painter.ClosePath();
        painter.fillColor = Color.black;
        painter.Fill();
    }

    // Choose edge point on rect based on direction vector (world-space)
    private Vector2 GetEdgePointWorld(Rect rect, Vector2 dir)
    {
        Vector2 absDir = new Vector2(Mathf.Abs(dir.x), Mathf.Abs(dir.y));

        // If rect has zero size, return center
        if (rect.width <= 0 || rect.height <= 0)
            return rect.center;

        if (absDir.x > absDir.y)
        {
            // horizontal edge
            if (dir.x > 0)
                return new Vector2(rect.xMax, rect.center.y); // right
            else
                return new Vector2(rect.xMin, rect.center.y); // left
        }
        else
        {
            // vertical edge
            if (dir.y > 0)
                return new Vector2(rect.center.x, rect.yMax); // top
            else
                return new Vector2(rect.center.x, rect.yMin); // bottom
        }
    }

    private bool IsFiniteVector(Vector2 v)
    {
        return float.IsFinite(v.x) && float.IsFinite(v.y) && Mathf.Abs(v.x) < 1e8f && Mathf.Abs(v.y) < 1e8f;
    }
}



