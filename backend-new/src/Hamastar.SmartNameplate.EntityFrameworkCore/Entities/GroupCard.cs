//-----
// <copyright file="GroupCard.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> SmartNameplate Team </author>
//-----

using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hamastar.SmartNameplate.Entities;

/// <summary>
/// 群組卡片關聯實體
/// </summary>
[Table("AppGroupCards")]
[Comment("群組卡片關聯表")]
public class GroupCard : Volo.Abp.Domain.Entities.BasicAggregateRoot<Guid>
{
    #region Properties

    /// <summary>
    /// 關聯 ID
    /// </summary>
    [Key]
    [Column("Id", TypeName = "uniqueidentifier")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Comment("關聯 ID")]
    public override Guid Id { get; protected set; }

    /// <summary>
    /// 群組 ID
    /// </summary>
    [Column("GroupId", TypeName = "uniqueidentifier")]
    [Comment("群組 ID")]
    [Required]
    public Guid GroupId { get; set; }

    /// <summary>
    /// 卡片 ID
    /// </summary>
    [Column("CardId", TypeName = "uniqueidentifier")]
    [Comment("卡片 ID")]
    [Required]
    public Guid CardId { get; set; }

    /// <summary>
    /// 建立時間
    /// </summary>
    [Column("CreationTime", TypeName = "datetime2(7)")]
    [Comment("建立時間")]
    [Required]
    public DateTime CreationTime { get; set; }

    /// <summary>
    /// 建立者
    /// </summary>
    [Column("CreatorUserId", TypeName = "uniqueidentifier")]
    [Comment("建立者")]
    [Required]
    public Guid CreatorUserId { get; set; }

    /// <summary>
    /// 最後修改時間
    /// </summary>
    [Column("LastModificationTime", TypeName = "datetime2(7)")]
    [Comment("最後修改時間")]
    [Required]
    public DateTime LastModificationTime { get; set; }

    /// <summary>
    /// 最後修改者
    /// </summary>
    [Column("LastModifierUserId", TypeName = "uniqueidentifier")]
    [Comment("最後修改者")]
    [Required]
    public Guid LastModifierUserId { get; set; }

    #endregion Properties

    #region Navigation Properties

    /// <summary>
    /// 群組
    /// </summary>
    public virtual Group Group { get; set; } = null!;

    /// <summary>
    /// 卡片
    /// </summary>
    public virtual Card Card { get; set; } = null!;

    #endregion Navigation Properties
} 