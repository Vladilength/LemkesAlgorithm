using System;
using System.Collections.Generic;
using System.Linq;

float[,] Mdata =
    {
    {2.0f,0.0f,0.0f,1.0f,-1.0f,1.0f},
    {-1.0f, 0.0f,0.0f,1.0f,-2.0f,1.0f},
    {3.0f,-1.0f,-1.0f,0.0f,0.0f,1.0f},
    {-2.0f,1.0f,2.0f,0.0f,0.0f,1.0f}
    };

Matrix<float> M = new Matrix<float>(
    Mdata
    );
// w1 = 1q + 2 *z0 + 3*z1 + 5z2
// z0 =  -1q/2 - w1/2  - 3/2 z1 - 5/2 z2
// wo = [q,z0,], - > [q,0,0,0,0]
//z0,w1
var w0 = new EqVariable();
w0.row_index = 0;
w0.initial_index = 0;
w0.is_basic = true;
var w1 = new EqVariable();
w1.row_index = 1;
w1.initial_index = 1;
w1.is_basic = true;

var w2 = new EqVariable();
w2.row_index = 2;
w2.initial_index = 2;
w2.is_basic = true;

var w3 = new EqVariable();
w3.row_index = 3;
w3.initial_index = 3;
w3.is_basic = true;

var z0 = new EqVariable();
z0.row_index = 0;
z0.variable_type = "z";
z0.col_index = 1;
z0.initial_index = 0;
z0.is_basic = false;

var z1 = new EqVariable();
z1.row_index = 0;
z1.variable_type = "z";
z1.col_index = 2;
z1.initial_index = 1;
z1.is_basic = false;

var z2 = new EqVariable();
z2.row_index = 0;
z2.variable_type = "z";
z2.col_index = 3;
z2.initial_index = 2;
z2.is_basic = false;

var z3 = new EqVariable();
z3.row_index = 0;
z3.variable_type = "z";
z3.col_index = 4;
z3.initial_index = 3;
z3.is_basic = false;

var z4 = new EqVariable();
z4.row_index = 0;
z4.variable_type = "z";
z4.col_index = 5;
z4.initial_index = 4;
z4.is_basic = false;
List<EqVariable> basics = new List<EqVariable>();
basics.Add(w0);
basics.Add(w1);
basics.Add(w2);
basics.Add(w3);
List<EqVariable> nonBasics = new List<EqVariable>();
nonBasics.Add(z0);
nonBasics.Add(z1);
nonBasics.Add(z2);
nonBasics.Add(z3);
nonBasics.Add(z4);
M.SetBasic(basics);
M.SetNonBasic(nonBasics);
M.LemkesAlgorithm();
// M.Swap(z4, w3);
M.Print();

// w1 -> z0 ()
//zo =
public class Matrix<T> where T : struct, IComparable,
IComparable<T>,
IConvertible,
IEquatable<T>,
IFormattable {
  public float[,] data;
  List<EqVariable> basics = new List<EqVariable>();
  List<EqVariable> nonBasics = new List<EqVariable>();
    // private int[,] a;

    public Matrix(float[,] data) {
    this.data = data;
  }

    // public Matrix(int[,] a)
    // {
    //     this.a = a;
    // }

    public void SetBasic(List<EqVariable> basics) {
    this.basics = basics;
  }
  public void SetNonBasic(List<EqVariable> nonBasics) {
    this.nonBasics = nonBasics;
  }
  // pivot basic with no basic
  public EqVariable Pivot(EqVariable basicVariable) {
    var enterVariable = new EqVariable();
    enterVariable.variable_type = basicVariable.variable_type == "w"
      ? "z"
      : "w";
    enterVariable.initial_index = basicVariable.initial_index;
    var _enterVariable = nonBasics.FirstOrDefault(x => 
    x.variable_type == enterVariable.variable_type && x.initial_index == enterVariable.initial_index);
    return _enterVariable;
  }
  
  public void Swap(EqVariable nonBasic, EqVariable basic) {
    //z0, w1 zo ->get column index okey w1 get row index
    // matrix[w1[rowIndex]] = [-q/z0.coeff, , - 1/ z.coeff, ];
    var rowIndex = basic.row_index;
    var colIndex = nonBasic.col_index;
    dynamic zCoeff = data[rowIndex, colIndex];
    for (int j = 0; j < data.GetLength(1); j++) {
      dynamic x = data[rowIndex, j];
      x = -x / zCoeff;
      if (j == colIndex) {
        data[rowIndex, j] = 1 / zCoeff;
      } else {
        data[rowIndex, j] = x;
      }
    }
    for (int i = 0; i < data.GetLength(0); i++) {
      //
      if (i == rowIndex) {
        continue;
      }
      dynamic _zCoeff = data[i, colIndex];
      for (int k = 0; k < data.GetLength(1); k++) {
        dynamic a;
        if (k == colIndex) {
          a = default(T);
        } else {
          a = data[i, k];
        }

        dynamic b = _zCoeff * data[rowIndex, k];
        data[i, k] = (T)a + b;
      }
    }
    //z0, w3
    // nonBasic.col_index = 2;
    // nonBasics = new EqVariable[0];
    // basic.col_index = 1;
    int basicIndex = basics.FindIndex(x => x == basic);
    int nonBasicIndex = nonBasics.FindIndex(x => x == nonBasic);

    var nonBasicCopy = new EqVariable();
    var basicCopy = new EqVariable();
    nonBasicCopy.variable_type = nonBasic.variable_type;
    nonBasicCopy.initial_index = nonBasic.initial_index;

    basicCopy.variable_type = basic.variable_type;
    basicCopy.initial_index = basic.initial_index;

    basics[basicIndex].variable_type = nonBasicCopy.variable_type;

    basics[basicIndex].initial_index = nonBasicCopy.initial_index;

    nonBasics[nonBasicIndex].variable_type = basicCopy.variable_type;
    nonBasics[nonBasicIndex].initial_index = basicCopy.initial_index;
  }
  public void LemkesAlgorithm() {
    // todo check trivial solution
    // swap last z with w with max negative value
    var rowIndex = 0;
    dynamic minQ = data[0, 0]; // q on first row
    for (int i = 0; i < data.GetLength(0); i++) {
      dynamic x = data[i, 0];
      if (x < minQ) {
        minQ = x;
        rowIndex = i;
      }
    }
    EqVariable w = basics.FirstOrDefault(x => x.row_index == rowIndex);
    // EqVariable zn = nonBasic
    EqVariable zn = null; // initialVariable
    int maxIndex = 1; // variables col index start after q variable
    foreach(var item in nonBasics) {
      if (item.col_index > maxIndex) {
        maxIndex = item.col_index;
        zn = item;
      }
    }

    var wCopy = new EqVariable();
    wCopy.initial_index = w.initial_index;
    wCopy.col_index = w.col_index;
    wCopy.row_index = w.row_index;
    wCopy.is_basic = w.is_basic;
    wCopy.variable_type = w.variable_type;

    Swap(zn, w);
    // find new z,v -> swap while variable zn non in non basic
    //w type ="w" -> z"index"
    //find min qi/vij
    // variable wi leaves dictionary-> zi enters dictionary
   /**/
    var enterVariable = new EqVariable();
    //Pivot(wCopy);  basic variable get return non basic
    enterVariable.variable_type = wCopy.variable_type == "w"
      ? "z"
      : "w";
    enterVariable.initial_index = wCopy.initial_index;
    // z3 in nonBasic
    var _enterVariable = nonBasics.FirstOrDefault(x => x.variable_type == enterVariable.variable_type && x.initial_index == enterVariable.initial_index);

    dynamic minRatio = Double.PositiveInfinity;
    int rowIndexToChange = -1;

    for (int _i = 0; _i < data.GetLength(0); _i++) {
      dynamic vjCoeff = data[_i, _enterVariable.col_index];
      // Console.WriteLine(vjCoeff);
      if (vjCoeff < 0) {
        dynamic q = data[_i, 0];
        dynamic qRatio = -q / vjCoeff;
        if (qRatio < minRatio) {
          minRatio = qRatio;
          rowIndexToChange = _i;
        }
      }
    }
    var basicToChange = basics.FirstOrDefault(x => x.row_index == rowIndexToChange);
    // get
    //   Swap(newZ, basicToChange) ->
  }
  public void Print() {
    var rowCount = data.GetLength(0);
    var colCount = data.GetLength(1);
    Console.Write(" ");
    Console.Write(" ");
    Console.Write(" ");
    for (int j = 0; j < colCount; j++) {
      var nonBasic = nonBasics.FirstOrDefault(x => x.col_index == j);
      if (nonBasic != null) {
        Console.Write(String.Format("{0}\t", nonBasic.variable_type + nonBasic.initial_index));
      } else {
        // q
        Console.Write(String.Format("{0}\t", "q"));
      }
    }
    Console.WriteLine();

    for (int row = 0; row < rowCount; row++) {
      // Console.WriteLine(basics[]);
      var basic = basics.FirstOrDefault(x => x.row_index == row);
      if (basic == null) {
        Console.Write(row);
        break;
      }
      Console.Write(basic.variable_type + basic.initial_index + " ");
      for (int col = 0; col < colCount; col++) 
        Console.Write(String.Format("{0}\t", data[row, col]));
      Console.WriteLine();
    }
  }
  public static Matrix<T> Multiply(Matrix<T> A, Matrix<T> B) {
    int rA = A.data.GetLength(0);
    int cA = A.data.GetLength(1);
    int rB = B.data.GetLength(0);
    int cB = B.data.GetLength(1);

    float[,] res = new float[rA, cB];
    if (cA != rB) {
      Console.WriteLine("Matrixes can't be multiplied!!");
    } else {
      float temp = 0.0f;

      for (int i = 0; i < rA; i++) {
        for (int j = 0; j < cB; j++) {
          temp = 0f;
          for (int k = 0; k < cA; k++) {
            dynamic x = A.data[i, k];
            dynamic y = B.data[k, j];

            temp += x * y;
          }
          res[i, j] = temp;
        }
      }
    }
    Matrix<T> C = new Matrix<T>(res);
    return C;
  }
}

public class EqVariable {
  public bool is_basic;
  // w or z
  public string variable_type = "w";
  public int initial_index = 0;
  public int col_index = 0;
  public int row_index = 0;
  //  public bool i
}
//     q, z0, z1, z2, z3
/* w0   1  2  3  4  5 7
   w1  -1  3  5  6  7 8
   w2  -1  3  5  6  7 8
   w3  -1  3  5  6  7 8
*/
//not echange