﻿using Common.Configurations.Packs;
using Common.Data.Models;

namespace Common.Data.Repositories.Base
{
    public interface ILevelRepository
    {
        LevelData GetLevelData(PackLevelCollection packLevelCollection, LevelPreviewData levelPreviewData);
    }
}