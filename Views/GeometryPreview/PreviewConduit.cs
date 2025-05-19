using Rhino.Display;
using Rhino.Geometry;
using System.Collections.Generic;
using Rhino.UI.Controls;

namespace Suimple.Views.GeometryPreview
{
    internal class PreviewConduit : DisplayConduit
    {
        internal List<Point3d> points = new List<Point3d>();
        internal List<Brep> breps = new List<Brep>();
        internal List<Mesh> meshes = new List<Mesh>();
        internal List<Curve> curves = new List<Curve>();

        internal System.Drawing.Color previewColor = System.Drawing.Color.Aqua;

        internal ViewportControl ViewportControl;

        internal bool DrawInRhino = false;

        internal bool DrawInControl = false;

        internal PreviewConduit(ViewportControl viewportControl) : base()
        {
            ViewportControl = viewportControl;
        }

        protected override void DrawForeground(Rhino.Display.DrawEventArgs e)
        {
            //ConduitDraw(e);
        }

        protected override void PostDrawObjects(Rhino.Display.DrawEventArgs e)
        {
            
            ConduitDraw(e);
        }

        protected override void DrawOverlay(DrawEventArgs e)
        {
            
            //ConduitDraw(e);
        }

        protected void ConduitDraw(DrawEventArgs e)
        {
            if (e.Viewport.Id == ViewportControl.Viewport.Id && !DrawInControl)
            {
                return;
            }
            if (e.Viewport.Id != ViewportControl.Viewport.Id && !DrawInRhino)
            {
                return;
            }

            var material = new DisplayMaterial(previewColor);

            foreach (Brep brep in breps)
            {
                e.Display.DrawBrepShaded(brep, material);
            }

            foreach (Curve curve in curves)
            {
                e.Display.DrawCurve(curve, System.Drawing.Color.BlueViolet);
            }

            foreach (Mesh mesh in meshes)
            {
                e.Display.DrawMeshShaded(mesh, material);
            }
        }

        internal void LoadGeometry(IEnumerable<Rhino.Geometry.GeometryBase> newGeometry)
        {
            curves = new List<Curve>();
            meshes = new List<Mesh>();
            breps = new List<Brep>();

            foreach (var geometry in newGeometry)
            {
                switch (geometry)
                {
                    case Curve c:
                        curves.Add(c);
                        break;
                    case Mesh m:
                        meshes.Add(m);
                        break;
                    case Brep b:
                        breps.Add(b);
                        break;
                }
            }
        }
    }
}
