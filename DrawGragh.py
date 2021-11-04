from matplotlib.colors import Colormap
import matplotlib.pyplot as plt
import numpy as np
from numpy.core.defchararray import title
import pandas as pd

fig = plt.figure(figsize=(10, 5), tight_layout=True)
plt.subplots_adjust(wspace=0.2, hspace=0.1)

FILE_NAME = "3FloorLv4_result_2021-11-02-15-43-40.csv"

# Input data
ax_Success = fig.add_subplot(
    221, title="Success?", xlabel="X pos", ylabel="Z pos")
Success_Data = pd.read_csv(FILE_NAME, usecols=[0, 1, 2], dtype="float")

ax_ReachStair1 = fig.add_subplot(
    222, title="Reached stair at Floor1", xlabel="X pos", ylabel="Z pos")
ReachStair1_Data = pd.read_csv(FILE_NAME, usecols=[0, 1, 3], dtype="float")

ax_ReachStair2 = fig.add_subplot(
    223, title="Reached stair at Floor2", xlabel="X pos", ylabel="Z pos")
ReachStair2_Data = pd.read_csv(FILE_NAME, usecols=[0, 1, 4], dtype="float")


# DataFrame
df_success = pd.DataFrame(Success_Data, columns=["X", "Z", "Success?"])
df_ReachStair1 = pd.DataFrame(ReachStair1_Data, columns=[
                              "X", "Z", "ReachStair_Floor1"])
df_ReachStair2 = pd.DataFrame(ReachStair2_Data, columns=[
                              "X", "Z", "ReachStair_Floor2"])

# Success?
colorList_1 = {-1: "gray", 0: "red", 1: "green"}
labelName_1 = {-1: "Error", 0: "Failure", 1: "Success"}

for s in set(df_success["Success?"]):
    df2_success = df_success[df_success["Success?"] == s]
    c = colorList_1[s]
    l = labelName_1[s]
    ax_Success.scatter(df2_success.X, df2_success.Z, s=10, color=c, label=l)

ax_Success.legend()


# ReachStair_Floor1
colorList_2 = {0: "gray", 1: "red", 2: "blue", 3: "yellow", 4: "green"}
labelName_2 = {0: "Failure", 1: "1-1", 2: "1-2", 3: "1-3", 4: "1-4"}

for r in set(df_ReachStair1["ReachStair_Floor1"]):
    df2_ReachStair1 = df_ReachStair1[df_ReachStair1["ReachStair_Floor1"] == r]
    c = colorList_2[r]
    l = labelName_2[r]
    ax_ReachStair1.scatter(
        df2_ReachStair1.X, df2_ReachStair1.Z, s=10, color=c, label=l)

ax_ReachStair1.legend()


# ReachStair_Floor2
colorList_3 = {0: "gray", 1: "red", 2: "blue", 3: "yellow", 4: "green"}
labelName_3 = {0: "Failure", 1: "2-1", 2: "2-2", 3: "2-3", 4: "2-4"}

for r in set(df_ReachStair2["ReachStair_Floor2"]):
    df2_ReachStair2 = df_ReachStair2[df_ReachStair2["ReachStair_Floor2"] == r]
    c = colorList_3[r]
    l = labelName_3[r]
    ax_ReachStair2.scatter(
        df2_ReachStair2.X, df2_ReachStair2.Z, s=10, color=c, label=l)

ax_ReachStair2.legend()

plt.subplots_adjust(left=0.030, right=0.995, top=0.965, bottom=0.040)
plt.show()

IMAGE_NAME = FILE_NAME[0:len(FILE_NAME) - 4] + ".png"
fig.savefig(IMAGE_NAME, facecolor="skyblue")
