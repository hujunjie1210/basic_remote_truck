using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace QyLost.UI.Chart
{
    public class Dashboard : Control
    {
        // 表盘与刻度适配， 但是指针还没有，指针还是按照40度执行
        private int offsetAngle = 60;//左右两侧各偏移的角度   
        private int offsetOuter = 20;//距离各侧边的偏移量
        private int roundSize = 10;// 进度条宽度

        private int indicatorCenterR = 16;//指针半径（中心点合并角度的半径）

        private int minWidth = 320;//最小宽度
        private int minHeight = 320;//最小高度

        private Pen pen = new Pen(Color.White, 1);//通用画笔
        private Brush brushInnerCircle;//内圆弧画笔
        private Brush brushInnerRound;//内芯圆画笔

        /// <summary>
        /// 初始化
        /// </summary>
        public Dashboard()
        {
            //初始化最小宽高
            this.Width = this.minWidth;
            this.Height = this.minHeight;
          
            //初始化相关画笔
            this.brushInnerCircle = new SolidBrush(this.innerBackground);
            this.brushInnerRound = new SolidBrush(this.innerRoundColor);
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
        }

        /// <summary>
        /// 处理Size改变事件
        /// </summary>
        /// <param name="e"></param>
        protected override void OnSizeChanged(EventArgs e)
        {
            //base.OnSizeChanged(e);
            //限制最小Size
            if (this.Width < this.minWidth)
            {
                this.Width = this.minWidth;
            }
            if (this.Height < this.minHeight)
            {
                this.Height = this.minHeight;
            }
        }

        #region 属性

        private decimal expected = 100;
        /// <summary>
        /// 期望值
        /// </summary>
        public decimal Expected
        {
            get
            {
                return this.expected;
            }
            set
            {
                if (value < this.real)
                {
                    return;
                }
                this.expected = value;
                this.Invalidate();
            }
        }

        private decimal real = 69;
        /// <summary>
        /// 实际值
        /// </summary>
        public decimal Real
        {
            get
            {
                return this.real;
            }
            set
            {
                if (value > this.expected)
                {
                    return;
                }
                this.real = value;
                this.Invalidate();
            }
        }
        #region 相关颜色
        private Color outerColor = Color.FromArgb(40, 151, 254);
        /// <summary>
        /// 外侧圆边颜色
        /// </summary>
        public Color OuterColor
        {
            get
            {
                return this.outerColor;
            }
            set
            {
                this.outerColor = value;
                this.Invalidate();
            }
        }

        private Color innerColor = Color.FromArgb(25, 106, 168);
        /// <summary>
        /// 内侧圆边颜色
        /// </summary>
        public Color InnerColor
        {
            get
            {
                return this.innerColor;
            }
            set
            {
                this.innerColor = value;
                this.Invalidate();
            }
        }

        private Color innerBackground = Color.FromArgb(0, 0, 0);
        /// <summary>
        /// 内圆弧背景色
        /// </summary>
        public Color InnerBackground
        {
            get
            {
                return this.innerBackground;
            }
            set
            {
                this.brushInnerCircle = new SolidBrush(value);
                this.innerBackground = value;
                this.Invalidate();
            }
        }

        private Color innerRoundColor = Color.FromArgb(1, 254, 254);
        /// <summary>
        /// 内芯圆颜色
        /// </summary>
        public Color InnerRoundColor
        {
            get
            {
                return this.innerRoundColor;
            }
            set
            {
                this.brushInnerRound = new SolidBrush(value);
                this.innerRoundColor = value;
                this.Invalidate();
            }
        }

        private Color bottomTitleColor = Color.Blue;
        /// <summary>
        /// 底部标题颜色
        /// </summary>
        public Color BottomTitleColor
        {
            get
            {
                return this.bottomTitleColor;
            }
            set
            {
                this.bottomTitleColor = value;
                this.Invalidate();
            }
        }

        private Color scaleRealColor = Color.FromArgb(135, 206, 235);
        /// <summary>
        /// 刻度值显示的完成值的颜色
        /// </summary>
        public Color ScaleRealColor
        {
            get
            {
                return this.scaleRealColor;
            }
            set
            {
                this.scaleRealColor = value;
                this.Invalidate();
            }
        }

        private Color scaleExpectedColor = Color.FromArgb(255, 105, 180);
        /// <summary>
        /// 刻度值显示的完成值的颜色
        /// </summary>
        public Color ScaleExpectedColor
        {
            get
            {
                return this.scaleExpectedColor;
            }
            set
            {
                this.scaleExpectedColor = value;
                this.Invalidate();
            }
        }

        private Color progressColor = Color.FromArgb(1, 254, 254);
        /// <summary>
        /// 进度条颜色
        /// </summary>
        public Color ProgressColor
        {
            get
            {
                return this.progressColor;
            }
            set
            {
                this.progressColor = value;
                this.Invalidate();
            }
        }

        #endregion

        private Font bottomTitleFont = new Font("微软雅黑", 16);
        /// <summary>
        /// 底部文字的的字体
        /// </summary>
        public Font BottomTitleFont
        {
            get
            {
                return this.bottomTitleFont;
            }
            set
            {
                bottomTitleFont = value;
                this.Invalidate();
            }
        }

        private Color indicatorColor = Color.FromArgb(1, 254, 254);
        /// <summary>
        /// 指针颜色
        /// </summary>
        public Color IndicatorColor
        {
            get
            {
                return this.indicatorColor;
            }
            set
            {
                this.indicatorColor = value;
                this.Invalidate();
            }
        }

        private int indicatorAngle = 10;
        /// <summary>
        /// 指针的角度
        /// </summary>
        public int IndicatorAngle
        {
            get
            {
                return indicatorAngle;
            }
            set
            {
                indicatorAngle = value;
                this.Invalidate();
            }
        }

        #region 重写的属性
        public override string Text
        {
            get => base.Text; set
            {
                base.Text = value;
                this.Invalidate();
            }
        }
        public override Color ForeColor
        {
            get => base.ForeColor; set
            {
                base.ForeColor = value;
                this.Invalidate();
            }
        }
        public override Font Font
        {
            get => base.Font; set
            {
                base.Font = value;
                this.Invalidate();
            }
        }
        #endregion

        #endregion

        /// <summary>
        /// 绘制控件
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaint(PaintEventArgs e)
        {
            //base.OnPaint(e);
            Graphics g = e.Graphics;
            //使绘图质量最高，即消除锯齿
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.CompositingQuality = CompositingQuality.HighQuality;

            //确定相关尺寸
            int R = Math.Min(this.Width, this.Height) / 2 - this.offsetOuter;//确定圆的半径
            int centerX = this.Width / 2;//中心点X轴
            int centerY = this.Height / 2;//中心点Y轴
            int startAngle = 90 + this.offsetAngle;//开始角度：从x轴正方向，逆时针旋转画弧
            int sweepAngle = 360 - this.offsetAngle * 2;//整个圆弧的角度

            decimal progress = this.real / this.expected;
            float processAngle = (float)(sweepAngle * progress);//到达进度值所占用的角度
            if (processAngle < 1)
            {
                processAngle = 1;
            }

            //1、绘制仪表盘指针： 没有跟着startAngle，sweepAngle
            DrawIndicator(g, R, (int)processAngle, startAngle, centerX, centerY) ;

            //2、绘制内侧圆:最中间的
            Rectangle rectangleCircle = new Rectangle(centerX - 20, centerY - 20, 40, 40);
            g.FillEllipse(this.brushInnerRound, rectangleCircle);

            //3、绘制最中心圆心
            Rectangle rectangleCircle1 = new Rectangle(centerX - 10, centerY - 10, 0, 0);
            g.FillEllipse(this.brushInnerCircle, rectangleCircle1);

            //4、绘制Real-Expect对比值
            string bottomStr = $"{this.real}/{this.expected}";
            SizeF bottomStrSize = g.MeasureString(bottomStr, this.bottomTitleFont);
            PointF bottomStrPosition = new PointF(centerX-40, centerY+100);//文字位置
            // g.DrawString(bottomStr, this.bottomTitleFont, new SolidBrush(this.bottomTitleColor), bottomStrPosition);


            // 5、绘制进度条
            int inner_width = 120;
            Rectangle rectangleInner = new Rectangle(
                this.Width / 2 - inner_width,
                this.Height / 2 - inner_width,
                inner_width * 2,
                inner_width * 2);//内圆所占范围
                g.DrawArc(GetPen(this.progressColor, this.roundSize), rectangleInner, startAngle, processAngle);

            // 格子状的进度条
            DrawScale(g, R, centerX, centerY, startAngle, (int)sweepAngle, rectangleInner);
        }

        /// <summary>
        /// 绘制仪表盘指针
        /// </summary>
        /// <param name="g"></param>
        /// <param name="R"></param>
        /// <param name="progressAngle"></param>
        /// <param name="centerX"></param>
        /// <param name="centerY"></param>
        private void DrawIndicator(Graphics g, int R, int progressAngle, int startAngel,int centerX, int centerY)
        {
            int leftPointAngel = (progressAngle + startAngel - indicatorAngle / 2);//计算出progressAngel向左侧偏移indicatorAngle/2的角度
            int rightPointAngel = (progressAngle + startAngel + indicatorAngle / 2);//计算出progressAngel向右侧偏移indicatorAngle/2的角度

            //计算出向左偏移Progress点-indicatorAngle/2的坐标
            double xLeft = centerX + indicatorCenterR * Math.Cos(leftPointAngel * Math.PI / 180);
            double yLeft = centerY + indicatorCenterR * Math.Sin(leftPointAngel * Math.PI / 180);

            //计算出向左偏移Progress点+indicatorAngle/2的坐标
            double xRight = centerX + indicatorCenterR * Math.Cos(rightPointAngel * Math.PI / 180);
            double yRight = centerY + indicatorCenterR * Math.Sin(rightPointAngel * Math.PI / 180);

            //指针的半径
            int indicatorR = R - R / 2;

            //计算指针终点的坐标
            double xTarget = centerX + indicatorR * Math.Cos((progressAngle + startAngel) * Math.PI / 180);
            double yTarget = centerY + indicatorR * Math.Sin((progressAngle + startAngel) * Math.PI / 180);
         
            //绘制闭合指针
            Brush brush = new SolidBrush(this.indicatorColor);
            g.FillPolygon(brush, new PointF[] {
                new PointF((float)xTarget,(float)yTarget),
                new PointF((float)xLeft,(float)yLeft),
                new PointF( centerX, centerY),
                new PointF( (float)xRight, (float)yRight),
                new PointF( (float)xTarget, (float)yTarget),
            });            
        }


        /// <summary>
        /// 绘制刻度值
        /// </summary>
        /// <param name="g"></param>
        /// <param name="R">圆的半径</param>
        /// <param name="centerX">圆的中心点X坐标</param>
        /// <param name="centerY">圆的中心点Y坐标</param>
        /// <param name="startAngle">开始角度（0点为圆的右侧边的中心点）</param>
        /// <param name="sweepAngle">以@startAngle为0点要覆盖的角度（0点为@startAngle点）</param>
        /// <param name="rectangleInner">内圆的空间参数</param>
        private void DrawScale(Graphics g, int R, int centerX, int centerY, int startAngle, int sweepAngle, Rectangle rectangleInner)
        {
            //DrawScale-1、定义相关尺寸信息

            //刻度线的开始点
            double scaleXStart = 0;
            double scaleYStart = 0;
            //刻度线的结束点
            double scaleXEnd = 0;
            double scaleYEnd = 0;
            int numScale = 90;
            decimal scaleVal = 0;//刻度值
            double scaleAngleItem = (double)sweepAngle / (double)(numScale-1);//每一个刻度所占据的角度
            int scaleSize = 20;//刻度线的长度
            float scaleOffset = 5f;//刻度线距离内圆的偏移量
            int intervalNum = 3;

            //DrawScale-2、定义相关画笔信息
            Pen penScale = GetPen(Color.White, 6);//刻度条画笔

            //DrawScale-3、开始正式绘制

            for (int i = 0; i < numScale; i++)
            {
                //计算开始点
                scaleXStart = centerX + (rectangleInner.Width / 2D - scaleSize) * Math.Cos((i * scaleAngleItem + startAngle) * Math.PI / 180);
                scaleYStart = centerY + (rectangleInner.Height / 2D - scaleSize) * Math.Sin((i * scaleAngleItem + startAngle) * Math.PI / 180);
                //计算结束点
                scaleXEnd = centerX + (rectangleInner.Width / 2D - scaleOffset) * Math.Cos((i * scaleAngleItem + startAngle) * Math.PI / 180);
                scaleYEnd = centerY + (rectangleInner.Height / 2D - scaleOffset) * Math.Sin((i * scaleAngleItem + startAngle) * Math.PI / 180);
                scaleVal = decimal.Round(this.expected / (numScale-1) * i, 2);

                //调换画笔
                if (scaleVal == 0 && this.real == 0)
                {
                    penScale.Color = this.scaleExpectedColor;
                }
                else
                {
                    if (scaleVal <= this.real)
                    {
                        penScale.Color = this.scaleRealColor;
                    }
                    else
                    {
                        penScale.Color = this.scaleExpectedColor;
                    }
                }
                if ((i > numScale/3-intervalNum && i < numScale/3+intervalNum)||(i>numScale*2/3-intervalNum && i < numScale * 2 / 3 + intervalNum))
                {
                    continue;
                }
                g.DrawLine(penScale, (float)scaleXStart, (float)scaleYStart, (float)scaleXEnd, (float)scaleYEnd);
            }
        }


        /// <summary>
        /// 设置并获取一个画笔（该画笔是复用的）
        /// </summary>
        /// <param name="color"></param>
        /// <param name="width"></param>
        /// <returns></returns>
        private Pen GetPen(Color color, int width = -1)
        {
            this.pen.Color = color;
            if (width != -1)
            {
                this.pen.Width = width;
            }
            return this.pen;
        }

        //解决控件批量更新时带来的闪烁
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams; cp.ExStyle |= 0x02000000;
                return cp;
            }
        }
    }
}
