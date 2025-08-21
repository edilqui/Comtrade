using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Axon.Comtrade.ViewModel
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Axon.Comtrade.Model;
    using Axon.UI.Components.TreeNode;

    public class TopologyTreeMapper
    {
        /// <summary>
        /// Convierte tu jerarquía de TopologyNodeModel a GenericTreeNodeModel para el TreeView
        /// </summary>
        public static ObservableCollection<GenericTreeNodeModel> MapToTreeView(List<TopologyNodeModel> topologyNodes)
        {
            var result = new ObservableCollection<GenericTreeNodeModel>();

            foreach (var topology in topologyNodes)
            {
                var treeNode = MapTopologyToTreeNode(topology, 0);
                result.Add(treeNode);
            }

            return result;
        }

        private static GenericTreeNodeModel MapTopologyToTreeNode(TopologyNodeModel topology, int level)
        {
            var treeNode = new GenericTreeNodeModel
            {
                Title = topology.Name,
                IconPath = GetTopologyIcon(topology.Type),
                Level = level,
                Tag = topology // ← Asociar tu objeto original
            };

            // Mapear sub-topologías (hijos)
            foreach (var childTopology in topology.Topologies)
            {
                var childTreeNode = MapTopologyToTreeNode(childTopology, level + 1);
                childTreeNode.Parent = treeNode;
                treeNode.Children.Add(childTreeNode);
            }

            // Mapear protocolos como hojas
            foreach (var protocol in topology.Protocols)
            {
                var protocolTreeNode = new GenericTreeNodeModel
                {
                    Title = protocol.Name,
                    IconPath = GetProtocolIcon(protocol.Type),
                    Level = level + 1,
                    Tag = protocol, // ← Asociar tu objeto protocol
                    Parent = treeNode
                };

                treeNode.Children.Add(protocolTreeNode);
            }

            return treeNode;
        }

        /// <summary>
        /// Obtiene el icono apropiado según el tipo de topología
        /// </summary>
        private static string GetTopologyIcon(string topologyType)
        {
            return TreeNodeIcons.None;
            //return topologyType?.ToLower() switch
            //{
            //    "subestacion" or "substation" => TreeNodeIcons.Server,
            //    "bahia" or "bay" => TreeNodeIcons.Computer,
            //    "dispositivos" or "devices" => TreeNodeIcons.Folder,
            //    "equipo" or "equipment" => TreeNodeIcons.Router,
            //    _ => TreeNodeIcons.Settings
            //};
        }

        /// <summary>
        /// Obtiene el icono apropiado según el tipo de protocolo
        /// </summary>
        private static string GetProtocolIcon(string protocolType)
        {
            switch (protocolType)
            {
                case "IEC-61850":
                case "FTP":
                case "TFTP":
                    return TreeNodeIcons.FileNetwork;
                default:
                    return TreeNodeIcons.None;
            }
        }

        /// <summary>
        /// Convierte de vuelta el TreeView a tu estructura de dominio
        /// </summary>
        public static List<TopologyNodeModel> MapFromTreeView(ObservableCollection<GenericTreeNodeModel> treeNodes)
        {
            var result = new List<TopologyNodeModel>();

            foreach (var treeNode in treeNodes)
            {
                if (treeNode.Tag is TopologyNodeModel topology)
                {
                    UpdateTopologyFromTreeNode(topology, treeNode);
                    result.Add(topology);
                }
            }

            return result;
        }

        private static void UpdateTopologyFromTreeNode(TopologyNodeModel topology, GenericTreeNodeModel treeNode)
        {
            // Actualizar propiedades básicas
            topology.Name = treeNode.Title;

            // Limpiar y reconstruir listas
            topology.Topologies.Clear();
            topology.Protocols.Clear();

            // Procesar hijos
            foreach (var childTreeNode in treeNode.Children)
            {
                if (childTreeNode.Tag is TopologyNodeModel childTopology)
                {
                    topology.Topologies.Add(childTopology);
                    UpdateTopologyFromTreeNode(childTopology, childTreeNode);
                }
                else if (childTreeNode.Tag is ProtocolNodeModel protocol)
                {
                    protocol.Name = childTreeNode.Title;
                    topology.Protocols.Add(protocol);
                }
            }
        }
    }
}
