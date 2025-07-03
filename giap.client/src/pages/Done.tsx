import done from '../assets/images/done.png'
import {useTranslation} from "react-i18next";
import {useEffect} from "react";
import {useLocation} from "react-router";

function Done() {
    const {t} = useTranslation();
    const location = useLocation();
    const identityProviderName = location.state?.identityProviderName || "";

    useEffect(() => {
        if (identityProviderName) {
            document.title = identityProviderName;
        }
    }, [identityProviderName]);

    return <>
        <header>
            <h1>{t("successHeader")}</h1>
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