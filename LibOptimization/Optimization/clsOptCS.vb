Imports LibOptimization.Util
Imports LibOptimization.MathUtil

Namespace Optimization
    ''' <summary>
    ''' Standard Cuckoo Search
    ''' </summary>
    ''' <remarks>
    ''' Features:
    '''  -Derivative free optimization algorithm.
    ''' 
    ''' Refference:
    ''' [1]Xin-She Yang, Suash Deb, "Cuckoo search via Lévy flights.", World Congress on Nature and Biologically Inspired Computing (NaBIC 2009). IEEE Publications. pp. 210–214. arXiv:1003.1594v1.
    ''' [2]Cuckoo Search (CS) Algorithm
    '''    http://www.mathworks.com/matlabcentral/fileexchange/29809-cuckoo-search--cs--algorithm
    ''' 
    ''' Implment:
    ''' N.Tomi(tomi.nori+github at gmail.com)
    ''' </remarks>
    Public Class clsOptCS : Inherits absOptimization
#Region "Member"
        '----------------------------------------------------------------
        'Common parameters
        '----------------------------------------------------------------
        ''' <summary>Max iteration count</summary>
        Public Overrides Property Iteration As Integer = 50000

        ''' <summary>
        ''' epsilon(Default:1e-8) for Criterion
        ''' </summary>
        Public Property EPS As Double = 0.000000001

        ''' <summary>
        ''' Use criterion
        ''' </summary>
        Public Property IsUseCriterion As Boolean = True

        ''' <summary>
        ''' higher N percentage particles are finished at the time of same evaluate value.
        ''' This parameter is valid is when IsUseCriterion is true.
        ''' </summary>
        Public Property HigherNPercent As Double = 0.8 'for IsCriterion()
        Private HigherNPercentIndex As Integer = 0 'for IsCriterion())

        '----------------------------------------------------------------
        'Parameters
        '----------------------------------------------------------------
        ''' <summary>
        ''' Population Size(Default:25)
        ''' </summary>
        Public Property PopulationSize As Integer = 25

        ''' <summary>
        ''' Discovery rate(Default:0.25)
        ''' </summary>
        Public Property PA As Double = 0.25

        ''' <summary>
        ''' Levy flight parameter(Default:1.5)
        ''' </summary>
        Public Property Beta As Double = 1.5

        'nest
        Private m_nests As New List(Of clsPoint)
        Private m_currentBest As clsPoint

#End Region

#Region "Constructor"
        ''' <summary>
        ''' Constructor
        ''' </summary>
        ''' <param name="ai_func">Optimize Function</param>
        ''' <remarks>
        ''' "n" is function dimension.
        ''' </remarks>
        Public Sub New(ByVal ai_func As absObjectiveFunction)
            Me.m_func = ai_func
        End Sub
#End Region

#Region "Public"
        ''' <summary>
        ''' Init
        ''' </summary>
        ''' <remarks></remarks>
        Public Overrides Sub Init()
            Try
                'init meber varibles
                Me.m_iteration = 0
                Me.m_nests.Clear()
                Me.m_error.Clear()

                'initial position
                For i As Integer = 0 To Me.PopulationSize - 1
                    Dim array = clsUtil.GenRandomPositionArray(Me.m_func, InitialPosition, Me.InitialValueRangeLower, Me.InitialValueRangeUpper)
                    Me.m_nests.Add(New clsPoint(Me.m_func, array))
                Next

                'Sort Evaluate
                Me.m_nests.Sort()

                'Detect HigherNPercentIndex
                Me.HigherNPercentIndex = CInt(Me.m_nests.Count * Me.HigherNPercent)
                If Me.HigherNPercentIndex = Me.m_nests.Count OrElse Me.HigherNPercentIndex >= Me.m_nests.Count Then
                    Me.HigherNPercentIndex = Me.m_nests.Count - 1
                End If
            Catch ex As Exception
                Me.m_error.SetError(True, clsError.ErrorType.ERR_INIT)
            End Try
        End Sub

        ''' <summary>
        ''' Do Iteration
        ''' </summary>
        ''' <param name="ai_iteration">Iteration count. When you set zero, use the default value.</param>
        ''' <returns>True:Stopping Criterion. False:Do not Stopping Criterion</returns>
        ''' <remarks></remarks>
        Public Overrides Function DoIteration(Optional ByVal ai_iteration As Integer = 0) As Boolean
            'Check Last Error
            If Me.IsRecentError() = True Then
                Return True
            End If

            'current best
            Me.m_currentBest = Me.m_nests(clsUtil.FindCurrentIndex(Me.m_nests))

            'levy flight parameter
            Dim sigma = (Me.Gamma(1 + Beta) * Math.Sin(Math.PI * Beta / 2) / (Me.Gamma((1 + Beta) / 2) * Beta * 2 ^ ((Beta - 1) / 2))) ^ (1 / Beta)

            'Do Iterate
            ai_iteration = If(ai_iteration = 0, Me.Iteration - 1, ai_iteration - 1)
            For iterate As Integer = 0 To ai_iteration
                'Evaluate
                Dim sortedEvalList = clsUtil.GetSortedEvalList(Me.m_nests)

                'check criterion
                If Me.IsUseCriterion = True Then
                    'higher N percentage particles are finished at the time of same evaluate value.
                    Dim bestEval = sortedEvalList(0).Eval
                    Dim nPercentEval = sortedEvalList(Me.HigherNPercentIndex).Eval
                    If clsUtil.IsCriterion(Me.EPS, bestEval, nPercentEval) Then
                        Return True
                    End If
                End If

                'Counting generation
                If Me.Iteration <= Me.m_iteration Then
                    Return True
                End If
                m_iteration += 1

                'get cuckoo
                Dim newNests As New List(Of clsPoint)(Me.PopulationSize)
                For i = 0 To Me.PopulationSize - 1
                    Dim s = Me.m_nests(i)
                    Dim newNest = New clsPoint(Me.m_func)
                    For j = 0 To Me.m_func.NumberOfVariable - 1
                        Dim u = clsUtil.NormRand(0) * sigma
                        Dim v = Math.Abs(clsUtil.NormRand(0))
                        Dim tempStep = u / Math.Pow(v, 1 / Beta)
                        Dim stepSize = 0.01 * tempStep * (s(j) - Me.m_currentBest(j))
                        Dim temp = s(j) + stepSize * clsUtil.NormRand(0)
                        newNest(j) = temp
                    Next
                    newNest.ReEvaluate()
                    newNests.Add(newNest) 'new nests
                Next

                'Find current best
                Dim candidateBest = newNests(clsUtil.FindCurrentIndex(newNests))
                If candidateBest.Eval < Me.m_currentBest.Eval Then
                    Me.m_currentBest = candidateBest.Copy()
                End If

                'replace
                For i = 0 To Me.PopulationSize - 1
                    If newNests(i).Eval < Me.m_nests(i).Eval Then
                        Me.m_nests(i) = newNests(i)
                    End If
                Next

                'Discovery and randomization
                newNests.Clear()
                Dim randPerm1 = clsUtil.RandomPermutaion(Me.PopulationSize)
                Dim randPerm2 = clsUtil.RandomPermutaion(Me.PopulationSize)
                For i = 0 To Me.PopulationSize - 1
                    Dim newNest = New clsPoint(Me.m_func)
                    For j = 0 To Me.m_func.NumberOfVariable - 1
                        If Me.m_rand.NextDouble() > Me.PA Then
                            newNest(j) = Me.m_nests(i)(j) + Me.m_rand.NextDouble() * (Me.m_nests(randPerm1(i))(j) - Me.m_nests(randPerm2(i))(j))
                        Else
                            newNest(j) = Me.m_nests(i)(j)
                        End If
                    Next
                    newNest.ReEvaluate()
                    newNests.Add(newNest)
                Next

                'Find current best
                candidateBest = newNests(clsUtil.FindCurrentIndex(newNests))
                If candidateBest.Eval < Me.m_currentBest.Eval Then
                    Me.m_currentBest = candidateBest.Copy()
                End If

                'replace
                For i = 0 To Me.PopulationSize - 1
                    If newNests(i).Eval < Me.m_nests(i).Eval Then
                        Me.m_nests(i) = newNests(i)
                    End If
                Next
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
                Return clsUtil.GetBestPoint(Me.m_nests, True)
            End Get
        End Property

        ''' <summary>
        ''' Get recent error
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function IsRecentError() As Boolean
            Return Me.m_error.IsError()
        End Function

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
                Return Me.m_nests
            End Get
        End Property
#End Region

#Region "Private"
        ''' <summary>
        ''' Log Gamma function
        ''' </summary>
        ''' <param name="x"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Refference:
        ''' C言語による最新アルゴリズム事典
        ''' </remarks>
        Private Function LogGamma(ByVal x As Double) As Double
            Dim LOG_2PI = 1.8378770664093456
            Dim N = 8

            Dim B0 = 1              '   /* 以下はBernoulli数 */
            Dim B1 = (-1.0 / 2.0)
            Dim B2 = (1.0 / 6.0)
            Dim B4 = (-1.0 / 30.0)
            Dim B6 = (1.0 / 42.0)
            Dim B8 = (-1.0 / 30.0)
            Dim B10 = (5.0 / 66.0)
            Dim B12 = (-691.0 / 2730.0)
            Dim B14 = (7.0 / 6.0)
            Dim B16 = (-3617.0 / 510.0)

            Dim v As Double = 1
            Dim w As Double

            While x < N
                v *= x
                x += 1
            End While
            w = 1 / (x * x)
            Return ((((((((B16 / (16 * 15)) * w + (B14 / (14 * 13))) * w _
                        + (B12 / (12 * 11))) * w + (B10 / (10 * 9))) * w _
                        + (B8 / (8 * 7))) * w + (B6 / (6 * 5))) * w _
                        + (B4 / (4 * 3))) * w + (B2 / (2 * 1))) / x _
                        + 0.5 * LOG_2PI - Math.Log(v) - x + (x - 0.5) * Math.Log(x)
        End Function

        ''' <summary>
        ''' Gamma function
        ''' </summary>
        ''' <param name="x"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Refference:
        ''' C言語による最新アルゴリズム事典
        ''' </remarks>
        Private Function Gamma(ByVal x As Double) As Double
            If (x < 0) Then
                Return Math.PI / (Math.Sin(Math.PI * x) * Math.Exp(LogGamma(1 - x)))
            End If
            Return Math.Exp(LogGamma(x))
        End Function
#End Region
    End Class
End Namespace
