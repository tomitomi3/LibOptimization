Imports LibOptimization.Util
Imports LibOptimization.MathTool

Namespace Optimization
    ''' <summary>
    ''' Nelder Mead Method wikipedia ver
    ''' </summary>
    ''' <remarks>
    ''' Features:
    '''  -Derivative free optimization algorithm.
    '''  -Also known as "Down hill simplex" or "simplex method".
    ''' 
    ''' Reffrence:
    ''' http://ja.wikipedia.org/wiki/Nelder-Mead%E6%B3%95
    ''' 
    ''' Implment:
    ''' N.Tomi(tomi.nori+github at gmail.com)
    ''' </remarks>
    <Serializable>
    Public Class clsOptNelderMeadWiki : Inherits absOptimization
#Region "Member"
        ''' <summary>Max iteration count(Default:5,000)</summary>
        Public Overrides Property Iteration As Integer = 5000

        ''' <summary>Epsilon(Default:0.000001) for Criterion</summary>
        Public Property EPS As Double = 0.000001

        '-------------------------------------------------------------------
        'Coefficient of Simplex Operator
        '-------------------------------------------------------------------
        ''' <summary>Refrection coeffcient(default:1.0)</summary>
        Public Property Refrection As Double = 1.0

        ''' <summary>Expantion coeffcient(default:2.0)</summary>
        Public Property Expantion As Double = 2.0

        ''' <summary>Contraction coeffcient(default:-0.5)</summary>
        Public Property Contraction As Double = -0.5

        ''' <summary>Shrink coeffcient(default:0.5)</summary>
        Public Property Shrink As Double = 0.5

        Private m_points As New List(Of clsPoint)
#End Region

#Region "Constructor"
        ''' <summary>
        ''' Constructor
        ''' </summary>
        ''' <param name="ai_func">Optimize Function</param>
        ''' <remarks></remarks>
        Public Sub New(ByVal ai_func As absObjectiveFunction)
            Me.m_func = ai_func
        End Sub
#End Region

#Region "Public"
        ''' <summary>
        ''' Init
        ''' </summary>
        ''' <remarks>
        ''' All vertexs are made at random.
        ''' </remarks>
        Public Overrides Sub Init()
            Try
                ' メンバ変数の初期化
                Me.m_error.Clear()
                Me.m_iteration = 0
                Me.m_points.Clear()

                ' Nelder–Mead法では、少なくとも2変数以上でなければならない
                If Me.m_func.NumberOfVariable < 2 Then
                    Me.m_error.SetError(True, clsError.ErrorType.ERR_INIT, "Number of variables must be at least 2 for Nelder–Mead simplex.")
                    Return
                End If

                ' InitialPosition のチェック
                Dim validInitial As Boolean = False
                If MyBase.InitialPosition IsNot Nothing Then
                    If MyBase.InitialPosition.Length = Me.m_func.NumberOfVariable Then
                        validInitial = True
                    Else
                        Throw New ArgumentException("The number of variavles in InitialPosition and objective function are different.")
                    End If
                End If

                ' 頂点数は (次元数 + 1)
                Dim simplexSize As Integer = Me.m_func.NumberOfVariable + 1
                Dim tempSimplex(simplexSize - 1)() As Double

                If validInitial Then
                    ' 初期位置を1頂点として採用（ここでは先頭頂点）
                    tempSimplex(0) = MyBase.InitialPosition
                    ' 残りの頂点は、初期位置を基準とした乱数生成で作成
                    For i As Integer = 1 To simplexSize - 1
                        tempSimplex(i) = clsUtil.GenRandomPositionArray(Me.m_func, MyBase.InitialPosition, Me.InitialValueRangeLower, Me.InitialValueRangeUpper)
                    Next
                Else
                    ' 初期位置が指定されていない場合は、すべて乱数生成で作成
                    For i As Integer = 0 To simplexSize - 1
                        tempSimplex(i) = clsUtil.GenRandomPositionArray(Me.m_func, Nothing, Me.InitialValueRangeLower, Me.InitialValueRangeUpper)
                    Next
                End If

                ' 生成された頂点（配列）から clsPoint オブジェクトを作成し、m_points に追加
                For i As Integer = 0 To simplexSize - 1
                    Me.m_points.Add(New clsPoint(Me.m_func, New List(Of Double)(tempSimplex(i))))
                Next

                ' 評価値に基づいて頂点をソート
                Me.m_points.Sort()

            Catch ex As Exception
                Me.m_error.SetError(True, clsError.ErrorType.ERR_INIT, ex.Message)
            End Try
        End Sub


        ''' <summary>
        ''' Do optimization
        ''' </summary>
        ''' <param name="ai_iteration">Iteration count. When you set zero, use the default value.</param>
        ''' <returns>True:Stopping Criterion. False:Do not Stopping Criterion</returns>
        ''' <remarks></remarks>
        Public Overrides Function DoIteration(Optional ByVal ai_iteration As Integer = 0) As Boolean
            'Check Last Error
            If Me.IsRecentError() = True Then
                Return True
            End If

            'Do Iterate
            If Me.Iteration <= m_iteration Then
                Return True
            Else
                ai_iteration = If(ai_iteration = 0, Iteration - m_iteration - 1, Math.Min(ai_iteration, Iteration - m_iteration) - 1)
            End If
            For iterate As Integer = 0 To ai_iteration
                'Counting generation
                m_iteration += 1

                'Check criterion
                Me.m_points.Sort()
                If Me.IsUseCriterion = True Then
                    If clsUtil.IsCriterion(Me.EPS, Me.m_points(0).Eval, Me.m_points(Me.m_points.Count - 1).Eval) Then
                        Return True
                    End If
                End If

                '-----------------------------------------------------
                'The following is optimization by Nelder-Mead Method.
                '-----------------------------------------------------
                'Calc centroid
                Dim centroid = Me.GetCentroid(m_points)

                'Reflection
                Dim refrection = Me.ModifySimplex(Me.WorstPoint, centroid, Me.Refrection)
                If BestPoint.Eval <= refrection.Eval AndAlso refrection.Eval < Worst2ndPoint.Eval Then
                    WorstPoint = refrection
                ElseIf refrection.Eval < BestPoint.Eval Then
                    'Expantion
                    Dim expantion = Me.ModifySimplex(Me.WorstPoint, centroid, Me.Expantion)
                    If expantion.Eval < refrection.Eval Then
                        WorstPoint = expantion
                    Else
                        WorstPoint = refrection
                    End If
                Else
                    'Contraction
                    Dim contraction = Me.ModifySimplex(WorstPoint, centroid, Me.Contraction)
                    If contraction.Eval < WorstPoint.Eval Then
                        WorstPoint = contraction
                    Else
                        'Reduction(Shrink) BestPoint以外を縮小
                        Me.CalcShrink(Me.Shrink)
                    End If
                End If
            Next

            Return False
        End Function

        ''' <summary>
        ''' Best result
        ''' </summary>
        ''' <returns>Best point class</returns>
        ''' <remarks></remarks>
        Public Overrides ReadOnly Property Result As clsPoint
            Get
                Return clsUtil.FindCurrentBestPointFromPoints(Me.m_points, True)
            End Get
        End Property

        ''' <summary>
        ''' All Result
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>
        ''' for Debug
        ''' </remarks>
        Public Overrides ReadOnly Property Results As List(Of clsPoint)
            Get
                Return Me.m_points
            End Get
        End Property
#End Region

#Region "Private Methods"
        ''' <summary>
        ''' Calc Centroid
        ''' </summary>
        ''' <param name="ai_vertexs"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function GetCentroid(ByVal ai_vertexs As List(Of clsPoint)) As clsPoint
            Dim ret As New clsPoint(ai_vertexs(0))

            Dim numVar As Integer = ai_vertexs(0).Count
            For i As Integer = 0 To numVar - 1
                Dim temp As Double = 0.0
                For numVertex As Integer = 0 To ai_vertexs.Count - 2 'Except Worst
                    temp += ai_vertexs(numVertex)(i)
                Next
                ret(i) = temp / (ai_vertexs.Count - 1)
            Next

            ret.ReEvaluate()
            Return ret
        End Function

        ''' <summary>
        ''' Simplex
        ''' </summary>
        ''' <param name="ai_tgt">Target vertex</param>
        ''' <param name="ai_base">Base vertex</param>
        ''' <param name="ai_coeff">Coeffcient</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' </remarks>
        Private Function ModifySimplex(ByVal ai_tgt As clsPoint, ByVal ai_base As clsPoint, ByVal ai_coeff As Double) As clsPoint
            Dim ret As New clsPoint(Me.m_func)
            For i As Integer = 0 To ret.Count - 1
                Dim temp As Double = ai_base(i) + ai_coeff * (ai_base(i) - ai_tgt(i))
                ret(i) = temp
            Next
            ret.ReEvaluate()
            Return ret
        End Function

        ''' <summary>
        ''' Shrink(Except best point)
        ''' </summary>
        ''' <param name="ai_coeff">Shrink coeffcient</param>
        ''' <remarks>
        ''' </remarks>
        Private Sub CalcShrink(ByVal ai_coeff As Double)
            For i As Integer = 1 To m_points.Count - 1 'expect BestPoint
                For j As Integer = 0 To Me.m_points(0).Count - 1
                    Dim temp = BestPoint(j) + ai_coeff * (Me.m_points(i)(j) - BestPoint(j))
                    m_points(i)(j) = temp
                Next
                m_points(i).ReEvaluate()
            Next
        End Sub
#End Region

#Region "Property(Private)"
        Private Property BestPoint() As clsPoint
            Get
                Return Me.m_points(0)
            End Get
            Set(ByVal value As clsPoint)
                Me.m_points(0) = value
            End Set
        End Property

        Private Property WorstPoint() As clsPoint
            Get
                Return Me.m_points(Me.m_points.Count - 1)
            End Get
            Set(ByVal value As clsPoint)
                Me.m_points(Me.m_points.Count - 1) = value
            End Set
        End Property

        Private Property Worst2ndPoint() As clsPoint
            Get
                Return Me.m_points(Me.m_points.Count - 2)
            End Get
            Set(ByVal value As clsPoint)
                Me.m_points(Me.m_points.Count - 2) = value
            End Set
        End Property
#End Region
    End Class

End Namespace