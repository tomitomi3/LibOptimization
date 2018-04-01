Imports LibOptimization.Util
Imports LibOptimization.MathUtil

Namespace Optimization
    ''' <summary>
    ''' Adaptive Differential Evolution Algorithm
    ''' JADE
    ''' </summary>
    ''' <remarks>
    ''' Features:
    '''  -Derivative free optimization algorithm.
    '''  -similar to GA algorithm.
    ''' 
    ''' Refference:
    '''  [1]Z.-H. Zhan, J. Zhang, Y. Li, and H. Chung, “JADE: Adaptive Differential Evolution With Optional External Archive” IEEE Trans. Systems, Man, and Cybernetics-Part B, vol. 39, no. 6, pp. 1362-1381, Dec. 2009. 
    '''  [2]阪井 節子,高濱 徹行, "パラメータの相関を考慮した適応型差分進化アルゴリズムJADEの改良", 不確実性の下での数理モデルとその周辺 Mathematical Model under Uncertainty and Related Topics RIMS 研究集会報告集
    '''  [3]田邊遼司, and 福永Alex. "自動チューナーを用いた異なる最大評価回数における Differential Evolution アルゴリズムのパラメータ設定の調査." 進化計算学会論文誌 6.2 (2015): 67-81.    ''' 
    '''  
    ''' Implment:
    ''' N.Tomi(tomi.nori+github at gmail.com)
    ''' </remarks>
    Public Class clsOptDEJADE : Inherits absOptimization
#Region "Member"
        '----------------------------------------------------------------
        'Common parameters
        '----------------------------------------------------------------
        ''' <summary>Max iteration count</summary>
        Public Overrides Property Iteration As Integer = 20000

        ''' <summary>epsilon(Default:1e-8) for Criterion</summary>
        Public Property EPS As Double = 0.000000001

        ''' <summary>Use criterion</summary>
        Public Property IsUseCriterion As Boolean = True

        ''' <summary>
        ''' higher N percentage particles are finished at the time of same evaluate value.
        ''' This parameter is valid is when IsUseCriterion is true.
        ''' </summary>
        Public Property HigherNPercent As Double = 0.8 'for IsCriterion()
        Private HigherNPercentIndex As Integer = 0 'for IsCriterion())

        ''' <summary>Upper bound(limit solution space)</summary>
        Public Property UpperBounds As Double() = Nothing

        ''' <summary>Lower bound(limit solution space)</summary>
        Public Property LowerBounds As Double() = Nothing

        '----------------------------------------------------------------
        'DE parameters
        '----------------------------------------------------------------
        ''' <summary>
        ''' Population Size(Default:100)
        ''' </summary>
        Public Property PopulationSize As Integer = 100

        ''' <summary>population</summary>
        Private m_parents As New List(Of clsPoint)

        ''' <summary>archive</summary>
        Private m_archive As New List(Of clsPoint)

        ''' <summary>Constant raio 0 to 1(Adaptive paramter for muF, muCR)(Default:0.1)</summary>
        Public Property C As Double = 0.1

        ''' <summary>Adapative cross over ratio(Default:0.5)</summary>
        Public Property MuCR As Double = 0.5

        ''' <summary>Adapative F(Default:0.5)</summary>
        Public Property MuF As Double = 0.5
#End Region

#Region "Constructor"
        ''' <summary>
        ''' Constructor
        ''' </summary>
        ''' <param name="ai_func">Objective Function</param>
        ''' <remarks>
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
                Me.m_parents.Clear()
                Me.m_error.Clear()
                Me.m_archive.Clear()

                'init muF, muCR
                MuCR = 0.5
                MuF = 0.5

                'bound check
                If UpperBounds IsNot Nothing AndAlso LowerBounds IsNot Nothing Then
                    If UpperBounds.Length <> Me.m_func.NumberOfVariable Then
                        Throw New Exception("UpperBounds.Length is different")
                    End If
                    If LowerBounds.Length <> Me.m_func.NumberOfVariable Then
                        Throw New Exception("LowerBounds.Length is different")
                    End If
                End If

                'generate population
                For i As Integer = 0 To Me.PopulationSize - 1
                    'initial position
                    Dim array = clsUtil.GenRandomPositionArray(Me.m_func, InitialPosition, Me.InitialValueRangeLower, Me.InitialValueRangeUpper)

                    'bound check
                    Dim tempPoint = New clsPoint(MyBase.m_func, array)
                    If UpperBounds IsNot Nothing AndAlso LowerBounds IsNot Nothing Then
                        clsUtil.LimitSolutionSpace(tempPoint, Me.LowerBounds, Me.UpperBounds)
                    End If

                    'save point
                    Me.m_parents.Add(tempPoint)
                Next

                'Sort Evaluate
                Me.m_parents.Sort()

                'Detect HigherNPercentIndex
                Me.HigherNPercentIndex = CInt(Me.m_parents.Count * Me.HigherNPercent)
                If Me.HigherNPercentIndex = Me.m_parents.Count OrElse Me.HigherNPercentIndex >= Me.m_parents.Count Then
                    Me.HigherNPercentIndex = Me.m_parents.Count - 1
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

            'Do Iterate
            ai_iteration = If(ai_iteration = 0, Me.Iteration - 1, ai_iteration - 1)
            For iterate As Integer = 0 To ai_iteration
                'Sort Evaluate
                Me.m_parents.Sort()

                'check criterion
                If Me.IsUseCriterion = True Then
                    'higher N percentage particles are finished at the time of same evaluate value.
                    If clsUtil.IsCriterion(Me.EPS, Me.m_parents(0).Eval, Me.m_parents(Me.HigherNPercentIndex).Eval) Then
                        Return True
                    End If
                End If

                'Counting generation
                If Me.Iteration <= Me.m_iteration Then
                    Return True
                End If
                m_iteration += 1

                '--------------------------------------------------------------------------------------------
                'DE process
                '--------------------------------------------------------------------------------------------
                'Mutation and Crossover
                Dim sumF As Double = 0.0
                Dim sumFSquare As Double = 0.0
                Dim sumCR As Double = 0.0
                Dim countSuccess As Integer = 0
                For i As Integer = 0 To Me.PopulationSize - 1
                    'update F
                    Dim F As Double = 0.0
                    While True
                        F = clsUtil.CauchyRand(MuF, 0.1)
                        If F < 0 Then
                            Continue While
                        End If
                        If F > 1 Then
                            F = 1.0
                        End If
                        Exit While
                    End While

                    'update CR 0 to 1
                    Dim CR As Double = clsUtil.NormRand(MuCR, 0.1)
                    If CR < 0 Then
                        CR = 0.0
                    ElseIf CR > 1 Then
                        CR = 1.0
                    End If

                    'pick pBest
                    Dim pbest As clsPoint = Nothing
                    Dim pBestIndex = CInt(Me.PopulationSize * Random.NextDouble())
                    If pBestIndex <= 2 Then
                        If Random.NextDouble() > 0.5 Then
                            pbest = Me.m_parents(0)
                        Else
                            pbest = Me.m_parents(1)
                        End If
                    ElseIf pBestIndex = Me.PopulationSize Then
                        pbest = m_parents(PopulationSize - 1) 'worst
                    Else
                        pbest = m_parents(Random.Next(0, pBestIndex))
                    End If

                    'xi,g
                    Dim xi = m_parents(i)

                    'pick xr1,g different parent without i
                    Dim tempIndex1 = clsUtil.RandomPermutaion(m_parents.Count, i)
                    Dim r1Index = tempIndex1(0)
                    Dim p1 As clsPoint = m_parents(r1Index)

                    'pick xr2~,g different parent without i, xr1,g
                    Dim sumIndex = m_parents.Count + m_archive.Count
                    Dim tempIndex2 = clsUtil.RandomPermutaion(0, sumIndex, {i, r1Index})
                    Dim r2Index = tempIndex2(0)
                    Dim p2 As clsPoint = Nothing
                    If r2Index >= m_parents.Count Then
                        r2Index = r2Index - m_parents.Count
                        p2 = m_archive(r2Index)
                    Else
                        p2 = m_parents(r2Index)
                    End If

                    'Mutation and Crossover
                    Dim child = New clsPoint(ObjectiveFunction)
                    Dim j = Random.Next() Mod ObjectiveFunction.NumberOfVariable
                    Dim D = ObjectiveFunction.NumberOfVariable - 1

                    'crossover
                    For k = 0 To ObjectiveFunction.NumberOfVariable - 1
                        If Random.NextDouble() < CR OrElse k = D Then
                            child(j) = xi(j) + F * (pbest(j) - xi(j)) + F * (p1(j) - p2(j))
                        Else
                            child(j) = xi(k)
                        End If
                        j = (j + 1) Mod ObjectiveFunction.NumberOfVariable 'next
                    Next
                    child.ReEvaluate() 'Evaluate child

                    'Limit solution space
                    clsUtil.LimitSolutionSpace(child, Me.LowerBounds, Me.UpperBounds)

                    'Survive
                    If child.Eval < m_parents(i).Eval Then
                        'add archive
                        m_archive.Add(m_parents(i).Copy)

                        'replace
                        m_parents(i) = child

                        'for adaptive parameter
                        sumF += F
                        sumFSquare += F ^ 2
                        sumCR += CR
                        countSuccess += 1
                    Else

                    End If
                Next 'population iteration

                'remove archive
                Dim removeCount = m_archive.Count - PopulationSize
                If removeCount > 0 Then
                    clsUtil.RandomizeArray(m_archive)
                    m_archive.RemoveRange(m_archive.Count - removeCount, removeCount)
                End If

                'calc muF, muCR
                If countSuccess > 0 Then
                    'μCR = (1 − c) · μCR + c · meanA(SCR)
                    MuCR = (1 - C) * MuCR + C * (sumCR / countSuccess)
                    'μF = (1 − c) · μF + c · meanL (SF)
                    MuF = (1 - C) * MuF + C * (sumFSquare / sumF)
                Else
                    MuCR = (1 - C) * MuCR
                    MuF = (1 - C) * MuF
                End If
                'Console.WriteLine("muF={0}, muCR={1}", MuF, MuCR)
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
                Return clsUtil.GetBestPoint(m_parents, True)
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
                Return Me.m_parents
            End Get
        End Property
#End Region

#Region "Private"
#End Region
    End Class
End Namespace
