Imports LibOptimization.Util
Imports LibOptimization.MathUtil

Namespace Optimization
    ''' <summary>
    ''' Nelder Mead Method
    ''' </summary>
    ''' <remarks>
    ''' Features:
    '''  -Derivative free optimization algorithm.
    '''  -Also known as "Down hill simplex" or "simplex method".
    ''' 
    ''' Reffrence:
    ''' William H. Press, Saul A. Teukolsky, William T. Vetterling, Brian P. Flannery,
    ''' "Numerical Recipes in C [日本語版] C言語による数値計算のレシピ", 平成19年第14刷, 株式会社技術評論社, pp295-299
    ''' 
    ''' Implment:
    ''' N.Tomi(tomi.nori+github at gmail.com)
    ''' </remarks>
    Public Class clsOptNelderMeadNR : Inherits absOptimization
#Region "Member"
        Private ReadOnly MAX_ITERATION As Integer = 5000
        Private ReadOnly EPS As Double = 0.000001

        'Coefficient of Simplex Operator
        Private ReadOnly COEFF_Refrection As Double = 1.0
        Private ReadOnly COEFF_Expantion As Double = 2.0
        Private ReadOnly COEFF_Contraction As Double = 0.5
        Private ReadOnly COEFF_Shrink As Double = 2.0

        'This Parameter to use when generate a vertex
        Private ReadOnly INIT_PARAM_RANGE As Double = 5

        Private m_points As New List(Of clsPoint)

        'ErrorManage
        Private m_error As New clsError
#End Region

#Region "Constructor"
        ''' <summary>
        ''' Constructor
        ''' </summary>
        ''' <param name="ai_func">Optimize Function</param>
        ''' <param name="ai_randomRange">Optional:random range(Default 5 => -5 to 5)</param>
        ''' <param name="ai_maxIteration">Optional:Iteration(default 5000)</param>
        ''' <param name="ai_eps">Optional:Eps(default:1e-6)</param>
        ''' <param name="ai_coeffRefrection">Optional:Refrection coeffcient(default:1.0)</param>
        ''' <param name="ai_coeffExpantion">Optional:Expantion coeffcient(default:2.0)</param>
        ''' <param name="ai_coeffContraction">Optional:Contraction coeffcient(default:0.5)</param>
        ''' <param name="ai_coeffShrink">Optional:Shrink coeffcient(default:2.0)</param>
        ''' <remarks></remarks>
        Public Sub New(ByVal ai_func As absObjectiveFunction, _
                       Optional ByVal ai_randomRange As Double = 5, _
                       Optional ByVal ai_maxIteration As Integer = 5000, _
                       Optional ByVal ai_eps As Double = 0.000001, _
                       Optional ByVal ai_coeffRefrection As Double = 1.0, _
                       Optional ByVal ai_coeffExpantion As Double = 2.0, _
                       Optional ByVal ai_coeffContraction As Double = 0.5, _
                       Optional ByVal ai_coeffShrink As Double = 2.0
                       )
            Me.m_func = ai_func

            Me.INIT_PARAM_RANGE = ai_randomRange
            Me.MAX_ITERATION = ai_maxIteration
            Me.EPS = ai_eps
            Me.COEFF_Refrection = ai_coeffRefrection
            Me.COEFF_Expantion = ai_coeffExpantion
            Me.COEFF_Contraction = ai_coeffContraction
            Me.COEFF_Shrink = ai_coeffShrink
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
                'Init meber varibles
                Me.m_error.Clear()
                Me.m_iteration = 0
                Me.m_points.Clear()

                'Make simplex from random vertex
                Dim tempSimplex()() As Double = Nothing
                ReDim tempSimplex(MyBase.m_func.NumberOfVariable)
                For i As Integer = 0 To tempSimplex.Length - 1
                    ReDim tempSimplex(i)(MyBase.m_func.NumberOfVariable - 1)
                    'Normal distribution
                    For j As Integer = 0 To m_func.NumberOfVariable - 1
                        tempSimplex(i)(j) = Math.Abs(2.0 * INIT_PARAM_RANGE) * m_rand.NextDouble() - INIT_PARAM_RANGE
                    Next
                Next

                Me.Init(tempSimplex)
            Catch ex As Exception
                Me.m_error.SetError(True, clsError.ErrorType.ERR_INIT, "")
            End Try
        End Sub

        ''' <summary>
        ''' Init
        ''' </summary>
        ''' <param name="ai_initPoint">Set 1 vertex</param>
        ''' <remarks>
        ''' Other vertexs are made at random.
        ''' </remarks>
        Public Overloads Sub Init(ByVal ai_initPoint() As Double)
            Try
                'init meber varibles
                Me.m_error.Clear()
                Me.m_iteration = 0
                Me.m_points.Clear()

                'Check var
                If ai_initPoint.Length <> Me.m_func.NumberOfVariable Then
                    Me.m_error.SetError(True, clsError.ErrorType.ERR_OPT_MAXITERATION)
                    Return
                End If

                'Make simplex from random vertex
                Dim tempSimplex()() As Double
                ReDim tempSimplex(MyBase.m_func.NumberOfVariable)
                tempSimplex(0) = ai_initPoint

                'Generate another vertexs
                For i As Integer = 1 To tempSimplex.Length - 1
                    ReDim tempSimplex(i)(MyBase.m_func.NumberOfVariable - 1)
                    'Normal distribution
                    For j As Integer = 0 To m_func.NumberOfVariable - 1
                        tempSimplex(i)(j) = Math.Abs(2.0 * INIT_PARAM_RANGE) * m_rand.NextDouble() - INIT_PARAM_RANGE
                    Next
                Next

                Me.Init(tempSimplex)
            Catch ex As Exception
                Me.m_error.SetError(True, clsError.ErrorType.ERR_INIT)
            End Try
        End Sub

        ''' <summary>
        ''' Init
        ''' </summary>
        ''' <param name="ai_initPoint"></param>
        ''' <remarks>
        ''' Set simplex
        ''' </remarks>
        Public Overloads Sub Init(ByVal ai_initPoint()() As Double)
            Try
                'init meber varibles
                Me.m_error.Clear()
                Me.m_iteration = 0
                Me.m_points.Clear()

                'Check number of variable
                If Me.m_func.NumberOfVariable < 2 Then
                    Me.m_error.SetError(True, clsError.ErrorType.ERR_INIT)
                    Return
                End If

                'Check Simplex
                'MEMO:Target function variable is the same as vertex of simplex.
                If ai_initPoint.Length <> (MyBase.m_func.NumberOfVariable + 1) Then
                    Me.m_error.SetError(True, clsError.ErrorType.ERR_INIT)
                    Return
                End If

                'Generate vertex
                For i As Integer = 0 To m_func.NumberOfVariable
                    Me.m_points.Add(New clsPoint(MyBase.m_func, New List(Of Double)(ai_initPoint(i))))
                Next

                'Sort Evaluate
                Me.m_points.Sort()

            Catch ex As Exception
                Me.m_error.SetError(True, clsError.ErrorType.ERR_INIT)
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
            ai_iteration = If(ai_iteration = 0, Me.MAX_ITERATION - 1, ai_iteration - 1)
            For iterate As Integer = 0 To ai_iteration
                'Sort Evaluate
                m_points.Sort()

                'Check criteorion
                If clsUtil.IsCriterion(Me.EPS, m_points(0).Eval, m_points(m_points.Count - 1).Eval) Then
                    Return True
                End If

                'Counting Iteration
                If MAX_ITERATION <= m_iteration Then
                    Me.m_error.SetError(True, clsError.ErrorType.ERR_OPT_MAXITERATION)
                    Return True
                End If
                m_iteration += 1

                'pick Best, worst, 2nd worst
                Dim bestVertex As clsPoint = m_points(0)
                Dim worstVertex As clsPoint = m_points(m_points.Count - 1)
                Dim worst2ndVertex As clsPoint = m_points(m_points.Count - 2)

                'Calc centroid
                Dim centroid As clsPoint = Me.GetCentroid(m_points)

                'Refrection
                Dim refrection As New clsPoint(centroid)
                For i As Integer = 0 To centroid.Count - 1
                    refrection(i) = 2.0 * centroid(i) - worstVertex(i)
                Next
                refrection.ReEvaluate()

                'comparison of refrection and worst
                If refrection.Eval >= worstVertex.Eval Then
                    Dim alpha As Double = 0.5
                    For i As Integer = 0 To centroid.Count - 1
                        Dim temp As Double = (1.0 - alpha) * centroid(i) - refrection(i)
                        refrection(i) = temp
                    Next
                    refrection.ReEvaluate()
                End If
                If refrection.Eval < worstVertex.Eval Then
                    m_points(m_points.Count - 1) = refrection
                End If

                If refrection.Eval <= bestVertex.Eval Then
                    'Expantion
                    Dim temp As clsPoint = Me.Ame(centroid, refrection)
                    If temp.Eval < worstVertex.Eval Then
                        m_points(m_points.Count - 1) = refrection
                    End If
                ElseIf refrection.Eval >= worst2ndVertex.Eval Then
                    'Contraction
                    Dim temp As clsPoint = Me.Ame(centroid, worst2ndVertex)
                    If temp.Eval < worstVertex.Eval Then
                        m_points(m_points.Count - 1) = refrection
                    End If
                    'Shrink
                    If refrection.Eval >= worst2ndVertex.Eval Then
                        Me.Shrink(m_points)
                    End If
                End If
            Next

            Return False
        End Function

        ''' <summary>
        ''' Contraction, Expantion
        ''' </summary>
        ''' <param name="ai_base1"></param>
        ''' <param name="ai_base2"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function Ame(ByVal ai_base1 As clsPoint, ByVal ai_base2 As clsPoint) As clsPoint
            Dim fac As Double = -1.0
            Dim fac1 As Double = (1.0 - fac) / ai_base1.Eval
            Dim fac2 As Double = fac1 - fac

            Dim ref As New clsPoint(ai_base1)
            For i As Integer = 0 To ai_base2.Count - 1
                ref(i) = ai_base1(i) * fac1 - ai_base2(i) * fac2
            Next
            ref.ReEvaluate()

            Return ref
        End Function

        ''' <summary>
        ''' Best result
        ''' </summary>
        ''' <returns>Best point class</returns>
        ''' <remarks></remarks>
        Public Overrides ReadOnly Property Result() As clsPoint
            Get
                Return Me.BestPoint
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
        Public Overrides ReadOnly Property ResultForDebug As List(Of clsPoint)
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
        ''' Shrink(All point replace)
        ''' </summary>
        ''' <param name="ai_allVertexs"></param>
        ''' <param name="ai_coeff">Shrink coeffcient.</param>
        ''' <remarks></remarks>
        Private Sub Shrink(ByRef ai_allVertexs As List(Of clsPoint), _
                           Optional ByVal ai_coeff As Double = 0.5)
            'Shrink. For 2 ≤ i ≤ n+1, define
            'xi = x1 + δ(xi −x1).
            Dim numVar As Integer = ai_allVertexs(0).Count

            'All point replace(Except bestpoint)
            For i As Integer = 1 To ai_allVertexs.Count - 1
                For j As Integer = 0 To numVar - 1
                    Dim temp As Double = ai_allVertexs(0)(j) + ai_coeff * (ai_allVertexs(i)(j) - ai_allVertexs(0)(j))
                    ai_allVertexs(i)(j) = temp
                Next
                ai_allVertexs(i).ReEvaluate()
            Next
        End Sub

        ''' <summary>
        ''' Get recent error infomation
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetLastErrorInfomation() As clsError.clsErrorInfomation
            Return Me.m_error.GetLastErrorInfomation()
        End Function

        ''' <summary>
        ''' Get recent error
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function IsRecentError() As Boolean
            Return Me.m_error.IsError()
        End Function
#End Region

#Region "Property"
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
                Return Me.m_points(m_points.Count - 1)
            End Get
            Set(ByVal value As clsPoint)
                Me.m_points(m_points.Count - 1) = value
            End Set
        End Property

        Private Property Worst2ndPoint() As clsPoint
            Get
                Return Me.m_points(m_points.Count - 2)
            End Get
            Set(ByVal value As clsPoint)
                Me.m_points(m_points.Count - 2) = value
            End Set
        End Property

        ''' <summary>
        ''' All Result
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>
        ''' for Debug, Experiment
        ''' </remarks>
        Public ReadOnly Property AllResult() As List(Of clsPoint)
            Get
                Return Me.m_points
            End Get
        End Property
#End Region
    End Class

End Namespace
