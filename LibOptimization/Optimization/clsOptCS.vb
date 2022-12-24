Imports LibOptimization.MathTool.RNG
Imports LibOptimization.Util

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
    <Serializable>
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

                'check initialposition
                If MyBase.InitialPosition IsNot Nothing Then
                    If MyBase.InitialPosition.Length = MyBase.m_func.NumberOfVariable Then
                        'nothing
                    Else
                        Throw New ArgumentException("The number of variavles in InitialPosition and objective function are different.")
                    End If
                End If

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
                Me.m_error.SetError(True, clsError.ErrorType.ERR_INIT, ex.Message)
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
            Me.m_currentBest = Me.m_nests(clsUtil.FindCurrentBestIndex(Me.m_nests))

            'levy flight parameter
            Dim sigma = (MathTool.MathUtil.Gamma(1 + Beta) * Math.Sin(Math.PI * Beta / 2) / (MathTool.MathUtil.Gamma((1 + Beta) / 2) * Beta * 2 ^ ((Beta - 1) / 2))) ^ (1 / Beta)

            'Do Iterate
            If Me.Iteration <= m_iteration Then
                Return True
            Else
                ai_iteration = If(ai_iteration = 0, Iteration - m_iteration - 1, Math.Min(ai_iteration, Iteration - m_iteration) - 1)
            End If
            For iterate As Integer = 0 To ai_iteration
                'Counting generation
                m_iteration += 1

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

                'get cuckoo
                Dim newNests As New List(Of clsPoint)(Me.PopulationSize)
                For i = 0 To Me.PopulationSize - 1
                    Dim s = Me.m_nests(i)
                    Dim newNest = New clsPoint(Me.m_func)
                    For j = 0 To Me.m_func.NumberOfVariable - 1
                        Dim u = RandomUtil.NormRand(0) * sigma
                        Dim v = Math.Abs(RandomUtil.NormRand(0))
                        Dim tempStep = u / Math.Pow(v, 1 / Beta)
                        Dim stepSize = 0.01 * tempStep * (s(j) - Me.m_currentBest(j))
                        Dim temp = s(j) + stepSize * RandomUtil.NormRand(0)
                        newNest(j) = temp
                    Next
                    newNest.ReEvaluate()
                    newNests.Add(newNest) 'new nests
                Next

                'Find current best
                Dim candidateBest = newNests(clsUtil.FindCurrentBestIndex(newNests))
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
                candidateBest = newNests(clsUtil.FindCurrentBestIndex(newNests))
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
                Return clsUtil.FindCurrentBestPointFromPoints(Me.m_nests, True)
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

#End Region
    End Class
End Namespace
