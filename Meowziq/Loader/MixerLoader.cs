﻿
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace Meowziq.Loader {
    /// <summary>
    /// Mixer のローダークラス
    /// </summary>
    public static class MixerLoader<T> {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // public static Methods [verb]

        /// <summary>
        /// Mixer を作成します
        ///     + ファイル読み込み
        /// </summary>
        public static void Build(string targetPath) {
            Core.Mixer<T>.Clear();
            if (!File.Exists(targetPath)) {
                return;
            }
            loadJson(targetPath).Mixer.Fader.ToList().Select(x => 
                new Core.Mixer<T>.Fader() {
                    Type = x.Type,
                    Vol = x.Vol,
                    Pan = Pan.Enum.Parse(x.Pan),
                    Mute = x.Mute
                }
            ).ToList().ForEach(x => Core.Mixer<T>.AddFader = x);
        }

        /// <summary>
        /// Mixer を作成します
        ///     + キャッシュした文字列
        /// </summary>
        public static void Build(Stream target) {
            if (target is null) {
                return;
            }
            loadJson(target).Mixer.Fader.ToList().Select(x =>
                new Core.Mixer<T>.Fader() {
                    Type = x.Type,
                    Vol = x.Vol,
                    Pan = Pan.Enum.Parse(x.Pan),
                    Mute = x.Mute
                }
            ).ToList().ForEach(x => Core.Mixer<T>.AddFader = x);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // private static Methods [verb]

        static Json loadJson(string targetPath) {
            using (var _stream = new FileStream(targetPath, FileMode.Open)) {
                var _serializer = new DataContractJsonSerializer(typeof(Json));
                return (Json) _serializer.ReadObject(_stream);
            }
        }

        static Json loadJson(Stream target) {
            var _serializer = new DataContractJsonSerializer(typeof(Json));
            return (Json) _serializer.ReadObject(target);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // inner Classes

        [DataContract]
        class Json {
            [DataMember(Name = "mixer")]
            public Mixer Mixer {
                get; set;
            }
        }

        [DataContract]
        class Mixer {
            [DataMember(Name = "fader")]
            public Fader[] Fader {
                get; set;
            }
        }

        [DataContract]
        class Fader {
            [DataMember(Name = "type")]
            public string Type {
                get; set;
            }
            [DataMember(Name = "vol")]
            public int Vol {
                get; set;
            }
            [DataMember(Name = "pan")]
            public string Pan {
                get; set;
            }
            [DataMember(Name = "mute")]
            public bool Mute {
                get; set;
            }
        }
    }
}