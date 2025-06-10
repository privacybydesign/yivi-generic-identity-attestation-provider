import done from '../assets/images/done.png'
import {useTranslation} from "react-i18next";

function Done() {
    const {t} = useTranslation();

    return <>
        <header>
            <h1>Attributes Loaded</h1>
        </header>
        <main id="main-content">
            <div className="imageContainer">
                <img src={done} alt="success"/>
                <p>{t("success")}</p>
            </div>
        </main>
    </>
}

export default Done;